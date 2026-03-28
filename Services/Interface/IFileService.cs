namespace RealWorld.Services.Interface;

using Microsoft.AspNetCore.Http;
using RealWorld.Common;

public interface IFileService
{
    Task<ServiceResult<string>> UploadImageAsync(IFormFile file);
    public string? GetAbsoluteFileUrl(string? relativeUrl);
}