using RealWorld.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace RealWorld.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TagsController : ApiControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(
        ITagService tagService
    )
    {
        _tagService = tagService;
    }

    /// <summary>
    /// Returns all tags, sorted alphabetically
    /// </summary>
    [HttpGet("")]
    public async Task<IActionResult> List()
    {
        var result = await _tagService.GetTagsAsync();
        return HandleResult(result);
    }
}