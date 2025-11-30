using AutoMapper;
using StarterKit.Application.Common.Exceptions;
using StarterKit.Application.Common.Interfaces;
using StarterKit.Application.Contracts;
using StarterKit.Application.DTOs.Auth;
using StarterKit.Application.DTOs.Users;
using StarterKit.Domain.Entities;
using StarterKit.Domain.Enums;

namespace StarterKit.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public AuthService(
        IUnitOfWork unitOfWork, 
        IPasswordHasher passwordHasher, 
        IJwtService jwtService, 
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.Password))
        {
            throw new ApiException(401, "Incorrect email or password");
        }

        var tokens = await GenerateAuthTokensAsync(user);

        return new AuthResponse
        {
            User = _mapper.Map<UserResponse>(user),
            Tokens = tokens
        };
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _unitOfWork.Users.ExistsByEmailAsync(request.Email))
        {
            throw new ApiException(400, "Email already taken");
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = _passwordHasher.HashPassword(request.Password),
            Role = Role.User
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();

        var tokens = await GenerateAuthTokensAsync(user);

        return new AuthResponse
        {
            User = _mapper.Map<UserResponse>(user),
            Tokens = tokens
        };
    }

    public async Task<AuthTokensDto> RefreshAuthAsync(string refreshToken)
    {
        // 1. Verify token signature is valid (handled by JwtService logic)
        var userId = _jwtService.ValidateToken(refreshToken);
        if (userId == null)
        {
            throw new ApiException(401, "Please authenticate");
        }

        // 2. Check if token exists in DB and is not blacklisted
        var tokenDoc = await _unitOfWork.Tokens.FindOneAsync(refreshToken, TokenType.Refresh);
        if (tokenDoc == null)
        {
            throw new ApiException(401, "Please authenticate");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(userId.Value);
        if (user == null)
        {
            throw new ApiException(401, "User not found");
        }

        // 3. Delete old refresh token
        await _unitOfWork.Tokens.DeleteByTokenAsync(refreshToken);
        
        // 4. Generate new pair
        var newTokens = await GenerateAuthTokensAsync(user);
        return newTokens;
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var tokenDoc = await _unitOfWork.Tokens.FindOneAsync(refreshToken, TokenType.Refresh);
        if (tokenDoc == null)
        {
            throw new ApiException(404, "Not found");
        }
        
        await _unitOfWork.Tokens.DeleteAsync(tokenDoc);
        await _unitOfWork.CompleteAsync();
    }
    
    public async Task ForgotPasswordAsync(string email)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(email);
        if (user == null)
        {
            throw new ApiException(404, "No users found with this email");
        }

        // Generate Reset Token
        var (tokenValue, expires) = await _jwtService.GenerateTokenAsync(user, TokenType.ResetPassword);
        
        var tokenEntity = new Token
        {
            TokenValue = tokenValue,
            UserId = user.Id,
            Type = TokenType.ResetPassword,
            Expires = expires
        };
        
        await _unitOfWork.Tokens.AddAsync(tokenEntity);
        await _unitOfWork.CompleteAsync();
        
        // In a real app, send email here
    }

    public async Task ResetPasswordAsync(string token, string newPassword)
    {
        var userId = _jwtService.ValidateToken(token);
        if (userId == null) throw new ApiException(401, "Password reset failed");
        
        var tokenDoc = await _unitOfWork.Tokens.FindOneAsync(token, TokenType.ResetPassword);
        if (tokenDoc == null) throw new ApiException(401, "Password reset failed");
        
        var user = await _unitOfWork.Users.GetByIdAsync(userId.Value);
        if (user == null) throw new ApiException(401, "Password reset failed");
        
        // Update Password
        user.Password = _passwordHasher.HashPassword(newPassword);
        await _unitOfWork.Users.UpdateAsync(user);
        
        // Remove all reset tokens for this user
        await _unitOfWork.Tokens.DeleteByUserAndTypeAsync(user.Id, TokenType.ResetPassword);
        await _unitOfWork.CompleteAsync();
    }

    // Helper
    private async Task<AuthTokensDto> GenerateAuthTokensAsync(User user)
    {
        var (accessToken, accessExpiry) = await _jwtService.GenerateTokenAsync(user, TokenType.Access);
        var (refreshToken, refreshExpiry) = await _jwtService.GenerateTokenAsync(user, TokenType.Refresh);

        // Save Refresh Token to DB
        var tokenEntity = new Token
        {
            TokenValue = refreshToken,
            UserId = user.Id,
            Type = TokenType.Refresh,
            Expires = refreshExpiry
        };

        await _unitOfWork.Tokens.AddAsync(tokenEntity);
        await _unitOfWork.CompleteAsync();

        return new AuthTokensDto
        {
            Access = new TokenDto { Token = accessToken, Expires = accessExpiry },
            Refresh = new TokenDto { Token = refreshToken, Expires = refreshExpiry }
        };
    }
}