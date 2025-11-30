using System.Linq.Expressions;
using StarterKit.Domain.Common;

namespace StarterKit.Application.Contracts;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}