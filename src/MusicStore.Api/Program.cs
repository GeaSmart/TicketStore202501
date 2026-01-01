using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using MusicStore.Api.Endpoints;
using MusicStore.Api.Filters;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Persistence;
using MusicStore.Persistence.Seeders;
using MusicStore.Repositories;
using MusicStore.Services.Implementations;
using MusicStore.Services.Interfaces;
using MusicStore.Services.Profiles;
using Serilog;
using Serilog.Events;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "log.txt");
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(logPath,
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: LogEventLevel.Information)
    .CreateLogger();

try
{
    builder.Logging.AddSerilog(logger);
    logger.Information($"LOG INITIALIZED in {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "NO ENV"}");

    //Options pattern register
    builder.Services.Configure<AppSettings>(builder.Configuration);

    var corsConfiguration = "MusicStoreCors";
    builder.Services.AddCors(setup =>
    {
        setup.AddPolicy(corsConfiguration, policy =>
        {
            policy.AllowAnyOrigin(); // Que cualquiera pueda consumir el API
            policy.AllowAnyHeader().WithExposedHeaders(new string[] { "TotalRecordsQuantity" });
            policy.AllowAnyMethod();
        });
    });

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add(typeof(FilterExceptions));
    });

    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            var response = new BaseResponse
            {
                Success = false,
                ErrorMessage = string.Join("; ", errors) // Une los mensajes de error en un solo string.
            };

            return new BadRequestObjectResult(response);
        };
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"));
    });

    // Identity
    builder.Services.AddIdentity<MusicStoreUserIdentity, IdentityRole>(
        policies =>
        {
            policies.Password.RequireDigit = true;
            policies.Password.RequiredLength = 6;
            policies.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
            {
                var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:JWTKey"] ??
                    throw new InvalidOperationException("JWT key not configured"));
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
    builder.Services.AddAuthorization();

    builder.Services.AddHttpContextAccessor();

    // Registering services
    builder.Services.AddScoped<IGenreRepository, GenreRepository>();
    builder.Services.AddScoped<IConcertRepository, ConcertRepository>();
    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
    builder.Services.AddScoped<ISaleRepository, SaleRepository>();
    builder.Services.AddScoped<IConcertService, ConcertService>();
    builder.Services.AddScoped<IGenreService, GenreService>();
    builder.Services.AddScoped<ISaleService, SaleService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddTransient<IFileStorage, FileStorageAzure>();

    builder.Services.AddTransient<UserDataSeeder>();
    builder.Services.AddTransient<GenreSeeder>();

    //Registering healthchecks
    builder.Services.AddHealthChecks()
        .AddCheck("selfcheck", () => HealthCheckResult.Healthy())
        .AddDbContextCheck<ApplicationDbContext>();

    // Register AutoMapper
    builder.Services.AddAutoMapper(config =>
    {
        config.AddProfile<ConcertProfile>();
        config.AddProfile<GenreProfile>();
        config.AddProfile<SaleProfile>();
    });

    var app = builder.Build();

    //if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseCors(corsConfiguration);

    app.MapReports();
    app.MapHomeEndpoints();

    app.MapControllers();


    // Aplicar migraciones y sembrar datos (asíncronamente)
    await ApplyMigrationsAndSeedDataAsync(app);

    //Configuring health checks
    app.MapHealthChecks("/healthcheck", new()
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "An unhandled exception occurred during the API initialization.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

static async Task ApplyMigrationsAndSeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (dbContext.Database.GetPendingMigrations().Any())
    {
        await dbContext.Database.MigrateAsync();
    }

    var userDataSeeder = scope.ServiceProvider.GetRequiredService<UserDataSeeder>();
    await userDataSeeder.SeedAsync();

    var genreSeeder = scope.ServiceProvider.GetRequiredService<GenreSeeder>();
    await genreSeeder.SeedAsync();
}