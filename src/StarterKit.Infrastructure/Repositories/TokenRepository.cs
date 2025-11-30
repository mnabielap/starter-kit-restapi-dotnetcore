using Microsoft.EntityFrameworkCore;
using StarterKit.Application.Contracts;
using StarterKit.Domain.Entities;
using StarterKit.Domain.Enums;
using StarterKit.Infrastructure.Data;

namespace StarterKit.Infrastructure.Repositories;

public class TokenRepository : GenericRepository<Token>, ITokenRepository
{
    public TokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Token?> FindOneAsync(string token, TokenType type, bool blacklisted = false)
    {
        return await _dbSet.FirstOrDefaultAsync(t => 
            t.TokenValue == token && 
            t.Type == type && 
            t.Blacklisted == blacklisted);
    }

    public async Task DeleteByTokenAsync(string token)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(t => t.TokenValue == token);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task DeleteByUserAndTypeAsync(int userId, TokenType type)
    {
        var tokens = await _dbSet.Where(t => t.UserId == userId && t.Type == type).ToListAsync();
        if (tokens.Any())
        {
            _dbSet.RemoveRange(tokens);
        }
    }
}