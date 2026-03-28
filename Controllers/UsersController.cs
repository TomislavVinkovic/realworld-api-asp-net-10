using RealWorld.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorld.Models.DTOs.Auth;
using RealWorld.Extensions;

namespace RealWorld.Controllers;

[Route("api/user")]
[ApiController]
public class UsersController : ApiControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var result = await _userService.LoginAsync(request.user);
        return HandleResult(result);
    }

    [HttpPost("")]
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        var result = await _userService.RegisterAsync(request.user);
        return HandleResult(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh(TokenRequest request)
    {
        var result = await _userService.RefreshAsync(request);
        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("logout")]
    public async Task<ActionResult> Logout()
    {
        var result = await _userService.LogoutAsync(User.GetRequiredUserId());
        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("")]
    public async Task<ActionResult> GetCurrentUser()
    {
        var currentAccessToken = HttpContext.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last() ?? "";

        var result = await _userService.GetCurrentUserAsync(currentAccessToken, User.GetRequiredUserId());
        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("edit")]
    public async Task<ActionResult> EditUser()
    {
        var currentAccessToken = HttpContext.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last() ?? "";

        var result = await _userService.GetCurrentUserAsync(currentAccessToken, User.GetRequiredUserId());
        return HandleResult(result);
    }

    [Authorize]
    [HttpPut("")]
    public async Task<ActionResult> UpdateUser ([FromForm] UpdateUserRequest request)
    {
        var result = await _userService.UpdateUserAsync(request.user, User.GetRequiredUserId());
        return HandleResult(result);
    }
}