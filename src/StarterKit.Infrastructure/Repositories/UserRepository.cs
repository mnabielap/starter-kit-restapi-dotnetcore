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
        string? search,
        string? scope,
        string? name, 
        Role? role, 
        string? sortBy, 
        int page, 
        int limit)
    {
        var query = _dbSet.AsQueryable();

        // 1. Specific Filtering
        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(u => u.Name.Contains(name));
        }

        if (role.HasValue)
        {
            query = query.Where(u => u.Role == role.Value);
        }

        // 2. Search & Scopes
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            var scopeLower = scope?.ToLower();

            switch (scopeLower)
            {
                case "name":
                    query = query.Where(u => u.Name.ToLower().Contains(searchLower));
                    break;
                case "email":
                    query = query.Where(u => u.Email.ToLower().Contains(searchLower));
                    break;
                case "id":
                    if (int.TryParse(search, out int idSearch))
                    {
                        query = query.Where(u => u.Id == idSearch);
                    }
                    else
                    {
                        // If searching by ID but input isn't int, return nothing
                        query = query.Where(u => false);
                    }
                    break;
                case "all":
                default:
                    if (int.TryParse(search, out int idAll))
                    {
                        // Match Name OR Email OR ID
                        query = query.Where(u => 
                            u.Name.ToLower().Contains(searchLower) || 
                            u.Email.ToLower().Contains(searchLower) ||
                            u.Id == idAll);
                    }
                    else
                    {
                        // Match Name OR Email
                        query = query.Where(u => 
                            u.Name.ToLower().Contains(searchLower) || 
                            u.Email.ToLower().Contains(searchLower));
                    }
                    break;
            }
        }

        var totalCount = await query.CountAsync();

        // 3. Sorting
        query = sortBy switch
        {
            "id:asc" => query.OrderBy(u => u.Id),
            "id:desc" => query.OrderByDescending(u => u.Id),
            
            "name:asc" => query.OrderBy(u => u.Name),
            "name:desc" => query.OrderByDescending(u => u.Name),
            
            "email:asc" => query.OrderBy(u => u.Email),
            "email:desc" => query.OrderByDescending(u => u.Email),
            
            "role:asc" => query.OrderBy(u => u.Role),
            "role:desc" => query.OrderByDescending(u => u.Role),
            
            "created_at:asc" => query.OrderBy(u => u.CreatedAt),
            "created_at:desc" => query.OrderByDescending(u => u.CreatedAt),
            
            _ => query.OrderByDescending(u => u.CreatedAt) // Default
        };

        // 4. Pagination
        var users = await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (users, totalCount);
    }
}