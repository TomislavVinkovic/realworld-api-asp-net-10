using RealWorld.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorld.Models.DTOs.Auth;
using Mapster;

namespace RealWorld.Controllers;

[Route("api/user")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IFileService _fileService;

    public UsersController(
        IUserService userService,
        IFileService fileService
    )
    {
        _userService = userService;
        _fileService = fileService;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        UserDto? user = await _userService.LoginAsync(request.user);
        if (user == null)
        {
            return Unauthorized("Invalid credentials.");
        }
        return Ok(new UserResponse(user));
    }

    [HttpPost("")]
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        UserDto? user = await _userService.RegisterAsync(request.user);
        return Ok(new UserResponse(user));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh(TokenRequest request)
    {
        UserDto? user = await _userService.RefreshAsync(request);
        if (user == null)
        {
            return BadRequest("Invalid or expired refresh token. Please log in again.");
        }
        return Ok(new UserResponse(user));
    }

    [Authorize]
    [HttpGet("logout")]
    public async Task<ActionResult> Logout()
    {
        var success = await _userService.LogoutAsync();
        if (!success)
        {
            return Unauthorized(); 
        }
        return NoContent();
    }

    [Authorize]
    [HttpGet("")]
    public async Task<ActionResult> GetCurrentUser()
    {
        var currentAccessToken = HttpContext.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last() ?? "";

        UserDto? user = await _userService.GetCurrentUserAsync(currentAccessToken);
        if (user == null) 
        {
            return NotFound();
        }
        return Ok(new UserResponse(user));
    }

    [Authorize]
    [HttpGet("edit")]
    public async Task<ActionResult> EditUser()
    {
        var currentAccessToken = HttpContext.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last() ?? "";

        UserDto? user = await _userService.GetCurrentUserAsync(currentAccessToken);
        if (user == null) return NotFound();

        var editDto = user.Adapt<EditUserDto>();
        return Ok(new EditUserResponse(editDto));
    }

    [Authorize]
    [HttpPut("")]
    public async Task<ActionResult> UpdateUser ([FromForm] UpdateUserRequest request)
    {
        string? relativeImagePath = null;
        if (request.user.Image?.Length > 0)
        {
            relativeImagePath = await _fileService.UploadImageAsync(request.user.Image);
        }

        var updateDto = request.user.Adapt<UpdateUserDto>();
        updateDto.Image = relativeImagePath;

        var user = await _userService.UpdateUserAsync(updateDto);

        return Ok(new UserResponse(user));
    }
}