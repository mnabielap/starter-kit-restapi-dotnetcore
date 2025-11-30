using Microsoft.AspNetCore.Mvc;
using StarterKit.Application.DTOs.Auth;
using StarterKit.Application.Services;

namespace StarterKit.Api.Controllers;

public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return StatusCode(201, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }

    [HttpPost("refresh-tokens")]
    public async Task<IActionResult> RefreshTokens(RefreshTokenRequest request)
    {
        var result = await _authService.RefreshAuthAsync(request.RefreshToken);
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequest request)
    {
        await _authService.LogoutAsync(request.RefreshToken);
        return NoContent();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request) // Need to define simple DTO
    {
        await _authService.ForgotPasswordAsync(request.Email);
        return NoContent();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        await _authService.ResetPasswordAsync(request.Token, request.Password);
        return NoContent();
    }
}

// Simple internal DTO for forgot password (can be moved to Application layer)
public class ForgotPasswordRequest { public string Email { get; set; } = string.Empty; }