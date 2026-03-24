using RealWorld.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorld.DTOs.Auth;

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
    public async Task<ActionResult> Login(DTOs.Auth.LoginRequest request)
    {
        var response = await _userService.LoginAsync(request.user);
        if (response == null) return Unauthorized("Invalid credentials.");

        return Ok(new { user = response });
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

    [HttpPost("")]
    public async Task<ActionResult> Register(DTOs.Auth.RegisterRequest request)
    {
        var response = await _userService.RegisterAsync(request.user);
        return Ok(new { user = response });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh(TokenRequest request)
    {
        var response = await _userService.RefreshAsync(request);
        if (response == null)
        {
            return BadRequest("Invalid or expired refresh token. Please log in again.");
        }
        return Ok(new { user = response });
    }

    [Authorize]
    [HttpGet("")]
    public async Task<ActionResult> GetCurrentUser()
    {
        // Extract the token directly from the header to hand it to the service
        var currentAccessToken = HttpContext.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last() ?? "";

        var response = await _userService.GetCurrentUserAsync(currentAccessToken);
        if (response == null) return NotFound();
        return Ok(new { user = response });
    }

    [Authorize]
    [HttpGet("edit")]
    public async Task<ActionResult> EditUser()
    {
        var currentAccessToken = HttpContext.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last() ?? "";

        var user = await _userService.GetCurrentUserAsync(currentAccessToken);
        if (user == null) return NotFound();

        var userDto = new EditUserDto
        {
            Email = user.Email,
            Bio = user.Bio,
            Image = user.Image,
            Username = user.Username
        };
        return Ok(new EditUserResponse(userDto));
    }

    [Authorize]
    [HttpPut("")]
    public async Task<ActionResult> UpdateUser ([FromForm] UpdateUserRequest request)
    {
        string? relativeImagePath = null;

        if (request.user.Image != null && request.user.Image.Length > 0)
        {
            try
            {
                relativeImagePath = await _fileService.UploadImageAsync(request.user.Image);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { errors = new { image = new[] { ex.Message } } });
            }
        }

        var updateDto = new UpdateUserDto
        {
            Email = request.user.Email,
            Username = request.user.Username,
            Password = request.user.Password,
            Bio = request.user.Bio,
            Image = relativeImagePath // Pass the relative path (or null if no upload)
        };

        try
        {
            var response = await _userService.UpdateUserAsync(updateDto);
            if (response == null)
            {
                return NotFound();   
            }
            return Ok(new { user = response });
        }
        catch (ArgumentException e)
        {
            // Catching duplicates for both email and username dynamically!
            return BadRequest(new { errors = new Dictionary<string, string[]> { { e.Message, new[] { "has already been taken" } } } });
        }
    }
}