using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Application.DTOs.Users;
using StarterKit.Application.Services;

namespace StarterKit.Api.Controllers;

[Authorize]
public class UsersController : BaseController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var user = await _userService.CreateUserAsync(request);
        return StatusCode(201, user);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers([FromQuery] UserFilterRequest filter)
    {
        var result = await _userService.GetUsersAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUser(int id)
    {
        if (!CanAccessUser(id))
        {
            return Forbid();
        }

        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
    {
        if (!CanAccessUser(id))
        {
            return Forbid();
        }

        var user = await _userService.UpdateUserAsync(id, request);
        return Ok(user);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        if (!CanAccessUser(id))
        {
            return Forbid();
        }

        await _userService.DeleteUserAsync(id);
        return NoContent();
    }

    private bool CanAccessUser(int resourceUserId)
    {
        var currentUser = User;

        if (currentUser.IsInRole("Admin"))
        {
            return true;
        }

        var currentUserIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(currentUserIdClaim, out int currentUserId))
        {
            return currentUserId == resourceUserId;
        }

        return false;
    }
}