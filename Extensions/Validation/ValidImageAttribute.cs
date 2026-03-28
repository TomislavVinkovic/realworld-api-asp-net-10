using System.ComponentModel.DataAnnotations;

namespace RealWorld.Extensions.Validation;

public class ValidImageAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // 1. If there's no file, it's valid (because image upload is optional)
        if (value is not IFormFile file || file.Length == 0)
        {
            return ValidationResult.Success;
        }

        // 2. Grab the configuration magically from the Validation Context
        var config = validationContext.GetService<IConfiguration>();
        if (config == null) return ValidationResult.Success; // Fallback

        var allowedExtensions = config.GetSection("FileUpload:AllowedExtensions").Get<string[]>();
        var maxSizeBytes = config.GetValue<long>("FileUpload:MaxFileSizeBytes");
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        // 3. Run the validation checks
        if (allowedExtensions == null || !allowedExtensions.Contains(extension))
        {
            return new ValidationResult("Invalid file type.");
        }

        if (file.Length > maxSizeBytes)
        {
            return new ValidationResult("File is too large.");
        }

        return ValidationResult.Success;
    }
}