using RealWorld.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace RealWorld.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {

        private readonly ITagService _tagService;

        public TagsController(
            ITagService tagService
        )
        {
            _tagService = tagService;
        }

        [HttpGet("")]
        public async Task<IActionResult> List()
        {
            var tagList = await _tagService.GetTagsAsync();
            return Ok(new TagListResponse(tagList.ToList()));
        }
    }
}
