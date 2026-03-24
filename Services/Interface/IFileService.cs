namespace RealWorld.Services.Interface;

using Microsoft.AspNetCore.Http;

public interface IFileService
{
    Task<string> UploadImageAsync(IFormFile file);
    public string? GetAbsoluteFileUrl(string? relativeUrl);
}