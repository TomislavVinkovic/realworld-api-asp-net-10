using RealWorld.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorld.Models.DTOs.Auth;
using RealWorld.Extensions;
using Mapster;

namespace RealWorld.Controllers;

[Route("api/user")]
[ApiController]
public class UsersController : ApiControllerBase
{
    private readonly IUserService _userService;
    private readonly IFileService _fileService;
    private readonly IConfiguration _configuration;

    public UsersController
    (
        IUserService userService,
        IFileService fileService,
        IConfiguration configuration
    )
    {
        _userService = userService;
        _fileService = fileService;
        _configuration = configuration;
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
        string? relativeImagePath = null;
        var file = request.user.Image;

        // 1. Just process the file if it exists
        if (file != null && file.Length > 0)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            using var stream = file.OpenReadStream();
            relativeImagePath = await _fileService.UploadAsync(stream, extension);
        }

        // 2. Map and pass to service
        var updateDto = request.user.Adapt<UpdateUserDto>();
        updateDto.Image = relativeImagePath;

        var result = await _userService.UpdateUserAsync(updateDto, User.GetRequiredUserId());
        return HandleResult(result);
    }
}