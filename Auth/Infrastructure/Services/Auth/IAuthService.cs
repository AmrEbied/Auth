using Auth.Infrastructure.Models;

namespace Auth.Infrastructure.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> GetTokenAsync(TokenRequestModel model);
        Task<AuthModel> ForgetPasswordAsync(ForgetPasswordModel model);
        Task<AuthModel> ChangePasswordAsync(ChangePasswordModel model);
        Task<AuthModel> SendCodeAsync(string email);
        Task<AuthModel> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string refreshToken);
        string UserId { get; }
    }
}
