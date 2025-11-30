using AutoMapper;
using StarterKit.Application.Common.Exceptions;
using StarterKit.Application.Common.Interfaces;
using StarterKit.Application.Common.Models;
using StarterKit.Application.Contracts;
using StarterKit.Application.DTOs.Users;
using StarterKit.Domain.Entities;
using StarterKit.Domain.Enums;

namespace StarterKit.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
    {
        if (await _unitOfWork.Users.ExistsByEmailAsync(request.Email))
        {
            throw new ApiException(400, "Email already taken");
        }

        var user = _mapper.Map<User>(request);
        user.Password = _passwordHasher.HashPassword(request.Password);

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<PagedResult<UserResponse>> GetUsersAsync(UserFilterRequest filter)
    {
        Role? roleFilter = null;
        if (!string.IsNullOrEmpty(filter.Role) && Enum.TryParse<Role>(filter.Role, true, out var parsedRole))
        {
            roleFilter = parsedRole;
        }

        var (users, totalCount) = await _unitOfWork.Users.FindAllAsync(
            filter.Name, 
            roleFilter, 
            filter.SortBy, 
            filter.Page, 
            filter.Limit);

        var userDtos = _mapper.Map<IEnumerable<UserResponse>>(users);
        var totalPages = (int)Math.Ceiling((double)totalCount / filter.Limit);

        return new PagedResult<UserResponse>
        {
            Results = userDtos,
            Page = filter.Page,
            Limit = filter.Limit,
            TotalPages = totalPages,
            TotalResults = totalCount
        };
    }

    public async Task<UserResponse> GetUserByIdAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), id);
        }

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), id);
        }

        if (!string.IsNullOrEmpty(request.Email) && await _unitOfWork.Users.ExistsByEmailAsync(request.Email, id))
        {
             throw new ApiException(400, "Email already taken");
        }

        if (!string.IsNullOrEmpty(request.Name)) user.Name = request.Name;
        if (!string.IsNullOrEmpty(request.Email)) user.Email = request.Email;
        
        if (!string.IsNullOrEmpty(request.Role) && Enum.TryParse<Role>(request.Role, true, out var parsedRole))
        {
            user.Role = parsedRole;
        }

        if (!string.IsNullOrEmpty(request.Password))
        {
            user.Password = _passwordHasher.HashPassword(request.Password);
        }

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<UserResponse>(user);
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), id);
        }

        await _unitOfWork.Users.DeleteAsync(user);
        await _unitOfWork.CompleteAsync();
    }
}