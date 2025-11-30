using StarterKit.Domain.Entities;
using StarterKit.Domain.Enums;

namespace StarterKit.Application.Common.Interfaces;

public interface IJwtService
{
    Task<(string Token, DateTime Expires)> GenerateTokenAsync(User user, TokenType type);
    int? ValidateToken(string token); // Returns UserId if valid
}