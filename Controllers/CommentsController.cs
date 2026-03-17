using dotnet_api_tutorial.Data;
using dotnet_api_tutorial.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_api_tutorial.Controllers
{
    [Route("api/articles")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController
        (
            ICommentService commentService
        )
        {
            _commentService = commentService;
        }

        [HttpGet("{slug}/comments")]
        public async Task<ActionResult> GetComments(string slug)
        {
            var comments = await _commentService.GetCommentsForArticleAsync(slug);
            if(comments == null)
            {
                return NotFound("Article not found");
            }

            return Ok(new CommentListResponse(comments));
        }

        [Authorize]
        [HttpPost("{slug}/comments")]
        public async Task<ActionResult> CreateComment(CreateCommentRequest request, string slug)
        {
            var comment = await _commentService.CreateAsync(request.comment, slug);
            if(comment == null)
            {
                return NotFound("Article not found");
            }
            return Ok(new CommentResponse(comment));
        }

        [Authorize]
        [HttpDelete("{slug}/comments/{id}")]
        public async Task<ActionResult> DeleteComment(string slug, int id)
        {
            bool deleted = await _commentService.DeleteAsync(id);
            if(!deleted)
            {
                return NotFound("Comment not found");
            }
            return Ok();
        }
    }
}
