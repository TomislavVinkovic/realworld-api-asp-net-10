using RealWorld.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorld.Models.DTOs.Comments;

namespace RealWorld.Controllers;

[Route("api/articles")]
[ApiController]
[Authorize]
public class CommentsController : ApiControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController
    (
        ICommentService commentService
    )
    {
        _commentService = commentService;
    }

    [AllowAnonymous]
    [HttpGet("{slug}/comments")]
    public async Task<ActionResult> GetComments(string slug)
    {
        var result = await _commentService.GetCommentsForArticleAsync(slug);
        return HandleResult(result);
    }

    [HttpPost("{slug}/comments")]
    public async Task<ActionResult> CreateComment(CreateCommentRequest request, string slug)
    {
        var result = await _commentService.CreateAsync(request.comment, slug);
        return HandleResult(result);
    }

    [HttpDelete("{slug}/comments/{id}")]
    public async Task<ActionResult> DeleteComment(string slug, int id)
    {
        var result = await _commentService.DeleteAsync(id);
        return HandleResult(result);
    }
}
