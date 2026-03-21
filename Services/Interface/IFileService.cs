namespace dotnet_api_tutorial.Services.Interface;

using Microsoft.AspNetCore.Http;

public interface IFileService
{
    Task<string> UploadImageAsync(IFormFile file);
}