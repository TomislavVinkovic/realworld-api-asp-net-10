namespace dotnet_api_tutorial.Services.Interface;

class FileService : IFileService
{
    public async Task<string> UploadImageAsync(IFormFile file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if(!allowedExtensions.Contains(extension))
        {
            throw new ArgumentException("Invalid file type. Only JPG, PNG, and BMP are allowed.");
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if(!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using(var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

       return $"/uploads/{uniqueFileName}";
    }
}