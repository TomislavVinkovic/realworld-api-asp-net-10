namespace RealWorld.Models.DTOs.Comments;

public record CommentListResponse(IEnumerable<CommentDto> comments);