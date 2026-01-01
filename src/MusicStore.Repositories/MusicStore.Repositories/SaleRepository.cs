using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MusicStore.Dto.Request;
using MusicStore.Entities;
using MusicStore.Entities.Info;
using MusicStore.Persistence;
using MusicStore.Repositories.Utils;
using System.Data;
using System.Linq.Expressions;

namespace MusicStore.Repositories;

public class SaleRepository : RepositoryBase<Sale>, ISaleRepository
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public SaleRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task CreateTransactionAsync()
    {
        await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);//block

    }
    public override async Task<int> AddAsync(Sale entity)
    {
        entity.SaleDate = DateTime.Now;
        var nextNumber = await context.Set<Sale>().CountAsync() + 1;
        entity.OperationNumber = $"{nextNumber:000000}";

        //add entity to context
        await context.AddAsync(entity);

        return entity.Id;
    }
    public override async Task UpdateAsync()
    {
        await context.Database.CommitTransactionAsync();
        await base.UpdateAsync();
    }
    public async Task RollBackAsync()
    {
        await context.Database.RollbackTransactionAsync();
    }
    public override async Task<Sale?> GetAsync(int id)
    {
        return await context.Set<Sale>()
            .Include(x => x.Customer)
            .Include(x => x.Concert)
            .ThenInclude(x => x.Genre)
            .Where(x => x.Id == id)
            .AsNoTracking()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync();
    }
    public async Task<ICollection<Sale>> GetAsync<TKey>(Expression<Func<Sale, bool>> predicate, Expression<Func<Sale, TKey>> orderBy, PaginationDTO pagination)
    {
        var queryable = context.Set<Sale>()
            .Include(x => x.Customer)
            .Include(x => x.Concert)
            .ThenInclude(x => x.Genre)
            .Where(predicate)
            .OrderBy(orderBy)
            .AsNoTracking()
            .AsQueryable();

        await httpContextAccessor.HttpContext.InsertarPaginacionHeader(queryable);
        var response = await queryable.Paginate(pagination).ToListAsync();
        return response;
    }

    public async Task<ICollection<ReportInfo>> GetSaleReportAsync(DateTime dateStart, DateTime dateEnd)
    {
        var query = context.Database
            .SqlQueryRaw<ReportInfo>(
                @"SELECT c.Title AS ConcertName, SUM(s.Total) AS Total
              FROM Musicales.Sale s
              INNER JOIN Musicales.Concert c ON c.Id = s.ConcertId
              WHERE s.SaleDate >= {0} AND s.SaleDate <= {1}
              GROUP BY c.Title",
                  dateStart, dateEnd);

        return await query.ToListAsync();
    }
}