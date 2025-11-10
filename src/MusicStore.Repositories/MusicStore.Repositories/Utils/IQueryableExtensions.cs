using MusicStore.Dto.Request;

namespace MusicStore.Repositories.Utils;

public static class IQueryableExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDTO paginacionDTO)
    {
        return queryable
            .Skip((paginacionDTO.Page - 1) * paginacionDTO.RecordsPerPage)
            .Take(paginacionDTO.RecordsPerPage);
    }
}