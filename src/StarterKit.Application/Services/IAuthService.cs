using StarterKit.Application.DTOs.Auth;
using StarterKit.Application.DTOs.Users;

namespace StarterKit.Application.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthTokensDto> RefreshAuthAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
    Task ForgotPasswordAsync(string email);
    Task ResetPasswordAsync(string token, string newPassword);
}