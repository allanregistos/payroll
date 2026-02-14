using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payroll.API.DTOs;
using Payroll.Application.Services;
using System.Security.Claims;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var (success, token, user) = await _authService.LoginAsync(request.Username, request.Password);

        if (!success || user == null)
            return Unauthorized(new { message = "Invalid username or password" });

        var userResponse = new UserResponse(
            user.UserId,
            user.Username,
            user.Email,
            user.FirstName ?? "",
            user.LastName ?? "",
            user.Role,
            user.IsActive,
            user.EmployeeId
        );

        var response = new LoginResponse(token, userResponse);

        return Ok(response);
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterRequest request)
    {
        var (success, message, user) = await _authService.RegisterAsync(
            request.Username,
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.Role,
            request.EmployeeId
        );

        if (!success || user == null)
            return BadRequest(new { message });

        var response = new UserResponse(
            user.UserId,
            user.Username,
            user.Email,
            user.FirstName ?? "",
            user.LastName ?? "",
            user.Role,
            user.IsActive,
            user.EmployeeId
        );

        return CreatedAtAction(nameof(GetProfile), new { id = user.UserId }, response);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var (success, message) = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

        if (!success)
            return BadRequest(new { message });

        return Ok(new { message });
    }

    [HttpGet("profile")]
    [Authorize]
    public ActionResult GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var employeeId = User.FindFirst("EmployeeId")?.Value;

        return Ok(new
        {
            userId,
            username,
            email,
            role,
            employeeId
        });
    }
}
