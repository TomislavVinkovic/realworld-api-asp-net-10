namespace RealWorld.Services.Interface;

using Microsoft.AspNetCore.Http;
using RealWorld.Common;

public interface IFileService
{
    Task<string> UploadAsync(Stream fileStream, string extension);
    public string? GetAbsoluteFileUrl(string? relativeUrl);
}