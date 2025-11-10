using MusicStore.Dto.Request;
using MusicStore.Entities;
using System.Linq.Expressions;

namespace MusicStore.Repositories;

public interface ISaleRepository : IRepositoryBase<Sale>
{
    Task CreateTransactionAsync();
    Task RollBackAsync();
    Task<ICollection<Sale>> GetAsync<TKey>(Expression<Func<Sale, bool>> predicate, Expression<Func<Sale, TKey>> orderBy, PaginationDTO pagination);
}