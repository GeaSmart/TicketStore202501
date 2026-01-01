using MusicStore.Dto.Request;
using MusicStore.Dto.Response;

namespace MusicStore.Services.Interfaces;

public interface ISaleService
{
    Task<BaseResponseGeneric<int>> AddAsync(string email, SaleRequestDto request);
    Task<BaseResponseGeneric<SaleResponseDto>> GetAsync(int id);
    Task<BaseResponseGeneric<ICollection<SaleResponseDto>>> GetAsync(SaleByDateSearchDto search, PaginationDTO pagination);
    Task<BaseResponseGeneric<ICollection<SaleResponseDto>>> GetAsync(string email, string title, PaginationDTO pagination);
    Task<BaseResponseGeneric<ICollection<SaleReportResponseDto>>> GetSaleReportAsync(DateTime dateStart, DateTime dateEnd);
}