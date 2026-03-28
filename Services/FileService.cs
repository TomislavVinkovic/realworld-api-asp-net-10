using RealWorld.Common;

namespace RealWorld.Services.Interface;

class FileService : IFileService
{
    private readonly IHttpContextService _httpContextService;
    public FileService(
        IHttpContextService httpContextService
    )
    {
        _httpContextService = httpContextService;
    }
    public string? GetAbsoluteFileUrl(string? relativeUrl)
    {
        var baseUrl = _httpContextService.GetBaseUrl();
        var fullImageUrl = relativeUrl;
        if (!string.IsNullOrEmpty(relativeUrl) && relativeUrl.StartsWith("/"))
        {
            fullImageUrl = $"{baseUrl}{relativeUrl}";
        }

        return fullImageUrl;
    }

    public async Task<string> UploadAsync(Stream fileStream, string extension)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if(!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using(var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }

       return $"/uploads/{uniqueFileName}";
    }
}