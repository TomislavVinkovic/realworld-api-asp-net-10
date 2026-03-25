namespace RealWorld.DTOs.Comments;

public record CommentListResponse(IEnumerable<CommentDto> comments);