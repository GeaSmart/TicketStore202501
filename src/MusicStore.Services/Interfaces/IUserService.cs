using MusicStore.Dto.Request;
using MusicStore.Dto.Response;

namespace MusicStore.Services.Interfaces;

public interface IUserService
{
    Task<BaseResponseGeneric<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<BaseResponseGeneric<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    Task<BaseResponse> RequestTokenToResetPasswordAsync(ResetPasswordRequestDto request);
    Task<BaseResponse> ResetPasswordAsync(NewPasswordRequestDto request);
    Task<BaseResponse> ChangePasswordAsync(string email, ChangePasswordRequestDto request);
}