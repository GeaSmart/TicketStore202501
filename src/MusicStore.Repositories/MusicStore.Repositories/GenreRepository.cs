using MusicStore.Entities;
using MusicStore.Persistence;

namespace MusicStore.Repositories;

public class GenreRepository : RepositoryBase<Genre>, IGenreRepository
{
    public GenreRepository(ApplicationDbContext context) : base(context)
    {
        
    }    
}