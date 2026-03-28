namespace RealWorld.Common;

public enum ServiceErrorType
{
    None,
    NotFound,
    Validation,
    Unauthorized,
    Forbidden,
    BadRequest,
    Conflict
}

public class ServiceResult<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }
    public ServiceErrorType ErrorType { get; init; }

    public static ServiceResult<T> Ok(T data) => new() { Success = true, Data = data };
    public static ServiceResult<T> Fail(string error) => new() { Success = false, Error = error };

    public static ServiceResult<T> NotFound(string message = "Resource not found") => 
        new() { Success = false, Error = message, ErrorType = ServiceErrorType.NotFound };

    public static ServiceResult<T> Unauthorized(string message = "You are not authorized to access this resource") => 
            new() { Success = false, Error = message, ErrorType = ServiceErrorType.Unauthorized };

    public static ServiceResult<T> BadRequest(string message = "Bad request") => 
            new() { Success = false, Error = message, ErrorType = ServiceErrorType.BadRequest };
}