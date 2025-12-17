using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicStore.Entities;

namespace MusicStore.Persistence.Seeders;

public class GenreSeeder
{
    private readonly IServiceProvider _serviceProvider;

    public GenreSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAsync()
    {
        // Obtener el contexto de la base de datos del servicio
        using (var context = _serviceProvider.GetRequiredService<ApplicationDbContext>())
        {
            // Definir los géneros que deseas añadir
            var listGenres = new List<Genre>
            {
                new Genre { Name = "Salsita" },
                new Genre { Name = "Rocas" }
            };

            // Obtener los nombres de los géneros que quieres añadir
            var genreNamesToAdd = listGenres.Select(g => g.Name).ToHashSet();

            // Obtener los nombres de los géneros existentes en la base de datos
            var existingGenreNames = await context.Set<Genre>()
                .Where(g => genreNamesToAdd.Contains(g.Name))
                .Select(g => g.Name)
                .ToListAsync();

            // Filtrar los géneros que no están en la base de datos
            var genresToAdd = listGenres
                .Where(g => !existingGenreNames.Contains(g.Name))
                .ToList();

            // Añadir los géneros que no existen en la base de datos
            if (genresToAdd.Any())
            {
                await context.Set<Genre>().AddRangeAsync(genresToAdd);
                await context.SaveChangesAsync();
            }
        }
    }
}