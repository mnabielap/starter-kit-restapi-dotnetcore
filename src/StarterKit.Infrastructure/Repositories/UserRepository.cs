using Microsoft.EntityFrameworkCore;
using StarterKit.Application.Contracts;
using StarterKit.Domain.Entities;
using StarterKit.Domain.Enums;
using StarterKit.Infrastructure.Data;

namespace StarterKit.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByEmailAsync(string email, int? excludeUserId = null)
    {
        if (excludeUserId.HasValue)
        {
            return await _dbSet.AnyAsync(u => u.Email == email && u.Id != excludeUserId.Value);
        }
        return await _dbSet.AnyAsync(u => u.Email == email);
    }

    public async Task<(IEnumerable<User> Users, int TotalCount)> FindAllAsync(
        string? name, 
        Role? role, 
        string? sortBy, 
        int page, 
        int limit)
    {
        var query = _dbSet.AsQueryable();

        // 1. Filtering
        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(u => u.Name.Contains(name));
        }

        if (role.HasValue)
        {
            query = query.Where(u => u.Role == role.Value);
        }

        var totalCount = await query.CountAsync();

        // 2. Sorting
        query = sortBy switch
        {
            "name:asc" => query.OrderBy(u => u.Name),
            "name:desc" => query.OrderByDescending(u => u.Name),
            "email:asc" => query.OrderBy(u => u.Email),
            "email:desc" => query.OrderByDescending(u => u.Email),
            "role:asc" => query.OrderBy(u => u.Role),
            "role:desc" => query.OrderByDescending(u => u.Role),
            _ => query.OrderByDescending(u => u.CreatedAt) // Default
        };

        // 3. Pagination
        var users = await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (users, totalCount);
    }
}