using StarterKit.Application.DTOs.Users;

namespace StarterKit.Application.DTOs.Auth;

public class AuthResponse
{
    public UserResponse User { get; set; } = null!;
    public AuthTokensDto Tokens { get; set; } = null!;
}

public class AuthTokensDto
{
    public TokenDto Access { get; set; } = null!;
    public TokenDto Refresh { get; set; } = null!;
}

public class TokenDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
}