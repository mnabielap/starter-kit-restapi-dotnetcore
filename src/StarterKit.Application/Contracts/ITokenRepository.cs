using StarterKit.Domain.Entities;
using StarterKit.Domain.Enums;

namespace StarterKit.Application.Contracts;

public interface ITokenRepository : IGenericRepository<Token>
{
    Task<Token?> FindOneAsync(string token, TokenType type, bool blacklisted = false);
    Task DeleteByTokenAsync(string token);
    Task DeleteByUserAndTypeAsync(int userId, TokenType type);
}