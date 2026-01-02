using StarterKit.Domain.Entities;
using StarterKit.Domain.Enums;

namespace StarterKit.Application.Contracts;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email, int? excludeUserId = null);
    
    Task<(IEnumerable<User> Users, int TotalCount)> FindAllAsync(
        string? search,
        string? scope,
        string? name, 
        Role? role, 
        string? sortBy, 
        int page, 
        int limit);
}