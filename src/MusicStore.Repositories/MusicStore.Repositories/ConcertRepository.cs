using MusicStore.Entities;
using MusicStore.Persistence;

namespace MusicStore.Repositories
{
    public class ConcertRepository : RepositoryBase<Concert>, IConcertRepository
    {
        public ConcertRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
