using dotnet_api_tutorial.DTOs;
using dotnet_api_tutorial.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_api_tutorial.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var response = await _userService.LoginAsync(request.user);
        
        if (response == null) return Unauthorized("Invalid credentials.");

        return Ok(new { user = response });
    }

    [HttpPost("")]
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        try
        {
            var response = await _userService.RegisterAsync(request.user);
            return Ok(new { user = response });
        }
        catch (ArgumentException ex)
        {
            // The service throws an ArgumentException with "email" if it's a duplicate.
            // This formats the error exactly how the RealWorld spec wants it!
            return BadRequest(new { errors = new Dictionary<string, string[]> { { ex.Message, new[] { "has already been taken" } } } });
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh(TokenRequest request)
    {
        var response = await _userService.RefreshAsync(request);
        
        if (response == null) return BadRequest("Invalid or expired refresh token. Please log in again.");

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
    [HttpPut("")]
    public async Task<ActionResult> UpdateUser(UpdateUserRequest request)
    {
        try
        {
            var response = await _userService.UpdateUserAsync(request.user);
            
            if (response == null) return NotFound();

            return Ok(new { user = response });
        }
        catch (ArgumentException e)
        {
            // Catching duplicates for both email and username dynamically!
            return BadRequest(new { errors = new Dictionary<string, string[]> { { e.Message, new[] { "has already been taken" } } } });
        }
    }
}