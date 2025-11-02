using MusicStore.Dto.Request;
using MusicStore.Dto.Response;

namespace MusicStore.Services.Interfaces
{
    public interface IConcertService
    {
        Task<BaseResponseGeneric<ICollection<ConcertResponseDto>>> GetAsync(string? title);
        Task<BaseResponseGeneric<ConcertResponseDto>> GetAsync(int id);
        Task<BaseResponseGeneric<int>> AddAsync(ConcertRequestDto request);
        Task<BaseResponse> UpdateAsync(int id, ConcertRequestDto request);
        Task<BaseResponse> DeleteAsync(int id);
        Task<BaseResponse> FinalizeAsync(int id);
    }
}
