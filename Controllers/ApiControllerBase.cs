using Microsoft.AspNetCore.Mvc;
using RealWorld.Common;

namespace RealWorld.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult HandleResult<T>(ServiceResult<T> result)
    {
        if (result.Success)
        {
            if (result.Data == null) return NoContent(); 
            return Ok(result.Data); 
        }

        var errorPayload = new { errors = new { body = new[] { result.Error } } };

        return result.ErrorType switch
        {
            ServiceErrorType.NotFound => NotFound(errorPayload),
            ServiceErrorType.Unauthorized => Unauthorized(errorPayload),
            ServiceErrorType.Forbidden => Forbid(),
            ServiceErrorType.Validation => UnprocessableEntity(errorPayload),
            ServiceErrorType.Conflict => Conflict(errorPayload),
            _ => BadRequest(errorPayload)
        };
    }
}