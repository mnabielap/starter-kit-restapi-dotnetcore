using StarterKit.Application.Common.Models;
using StarterKit.Application.DTOs.Users;

namespace StarterKit.Application.Services;

public interface IUserService
{
    Task<UserResponse> CreateUserAsync(CreateUserRequest request);
    Task<PagedResult<UserResponse>> GetUsersAsync(UserFilterRequest filter);
    Task<UserResponse> GetUserByIdAsync(int id);
    Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request);
    Task DeleteUserAsync(int id);
}