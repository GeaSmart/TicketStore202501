using AutoMapper;
using Microsoft.Extensions.Logging;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Repositories;
using MusicStore.Services.Interfaces;

namespace MusicStore.Services.Implementations;

public class SaleService : ISaleService
{
    private readonly ISaleRepository repository;
    private readonly ILogger<SaleService> logger;
    private readonly IMapper mapper;
    private readonly IConcertRepository concertRepository;
    private readonly ICustomerRepository customerRepository;

    public SaleService(ISaleRepository repository, ILogger<SaleService> logger, IMapper mapper,
        IConcertRepository concertRepository, ICustomerRepository customerRepository)
    {
        this.repository = repository;
        this.logger = logger;
        this.mapper = mapper;
        this.concertRepository = concertRepository;
        this.customerRepository = customerRepository;
    }
    public async Task<BaseResponseGeneric<int>> AddAsync(string email, SaleRequestDto request)
    {
        var response = new BaseResponseGeneric<int>();

        try
        {
            await repository.CreateTransactionAsync();
            var entity = mapper.Map<Sale>(request);

            var customer = await customerRepository.GetByEmailAsync(email);
            if (customer is null)
            {
                throw new InvalidOperationException(
                    $"El cliente con el email {email} no esta registrado. Por favor registrese antes de comprar.");
                //customer = new Customer()
                //{
                //    Email = request.Email,
                //    FullName = request.FullName
                //};
                //customer.Id = await customerRepository.AddAsync(customer);
            }
            // throw new Exception($"Excepcion a propósito para validar la transacción");
            entity.CustomerId = customer.Id;

            var concert = await concertRepository.GetAsync(request.ConcertId);
            if (concert is null)
                throw new Exception($"El concierto con el Id {request.ConcertId} no existe.");

            if (DateTime.Today > concert.DateEvent)
                throw new InvalidOperationException(
                    $"No se pueden comprar tickets para el concierto {concert.Title} porque ya pasó");

            if (concert.Finalized)
                throw new Exception($"El concierto con el Id {request.ConcertId} ya finalizó.");

            entity.Total = entity.Quantity * (decimal)concert.UnitPrice;

            await repository.AddAsync(entity);
            await repository.UpdateAsync();

            response.Data = entity.Id;
            response.Success = true;

            logger.LogInformation("Se creo correctamente la venta para {email}", email);
        }
        catch (InvalidOperationException ex)
        {
            await repository.RollBackAsync();
            response.ErrorMessage = ex.Message;

            logger.LogWarning(ex, "{ErrorMessage}", response.ErrorMessage);
        }
        catch (Exception ex)
        {
            await repository.RollBackAsync();
            response.ErrorMessage = "Error al crear la venta";
            logger.LogError(ex, "{ErrorMessage} {Message}", response.ErrorMessage, ex.Message);
        }

        return response;
    }

    public async Task<BaseResponseGeneric<SaleResponseDto>> GetAsync(int id)
    {
        var response = new BaseResponseGeneric<SaleResponseDto>();

        try
        {
            var sale = await repository.GetAsync(id);
            response.Data = mapper.Map<SaleResponseDto>(sale);
            response.Success = response.Data != null;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al seleccionar la venta";
            logger.LogError(ex, "{ErrorMessage} {Message}", response.ErrorMessage, ex.Message);
        }

        return response;
    }
    public async Task<BaseResponseGeneric<ICollection<SaleResponseDto>>> GetAsync(SaleByDateSearchDto search, PaginationDTO pagination)
    {
        var response = new BaseResponseGeneric<ICollection<SaleResponseDto>>();

        try
        {
            DateTime? dateInit = search.DateStart is not null ? Convert.ToDateTime(search.DateStart) : null;
            DateTime? dateEnd = search.DateEnd is not null ? Convert.ToDateTime(search.DateEnd) : null;

            var data = await repository.GetAsync(
                predicate: s =>
                        (dateInit == null || s.SaleDate >= dateInit) &&
                        (dateEnd == null || s.SaleDate <= dateEnd),
                orderBy: x => x.OperationNumber,
                pagination);

            response.Data = mapper.Map<ICollection<SaleResponseDto>>(data);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al filtrar las ventas por fecha.";
            logger.LogError(ex, "{ErrorMessage} {Message}", response.ErrorMessage, ex.Message);
        }

        return response;

    }

    public async Task<BaseResponseGeneric<ICollection<SaleResponseDto>>> GetAsync(string email, string title, PaginationDTO pagination)
    {
        var response = new BaseResponseGeneric<ICollection<SaleResponseDto>>();
        try
        {
            var data = await repository.GetAsync(
                predicate: s => s.Customer.Email == email && s.Concert.Title.Contains(title ?? string.Empty),
                orderBy: x => x.SaleDate,
                pagination);

            response.Data = mapper.Map<ICollection<SaleResponseDto>>(data);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al filtrar las ventas del usuario por título.";
            logger.LogError(ex, "{ErrorMessage} {Message}", response.ErrorMessage, ex.Message);
        }
        return response;
    }

    public async Task<BaseResponseGeneric<ICollection<SaleReportResponseDto>>> GetSaleReportAsync(DateTime dateStart, DateTime dateEnd)
    {
        var response = new BaseResponseGeneric<ICollection<SaleReportResponseDto>>();
        try
        {
            // Codigo
            var list = await repository.GetSaleReportAsync(dateStart, dateEnd);
            response.Data = mapper.Map<ICollection<SaleReportResponseDto>>(list);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = "Error al obtener los datos del reporte";
            logger.LogError(ex, "{ErrorMessage} {Message}", response.ErrorMessage, ex.Message);
        }
        return response;
    }
}