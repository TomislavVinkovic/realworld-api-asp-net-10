using FluentValidation;
using RealWorld.Models.DTOs.Auth;

namespace RealWorld.Models.Validators;

// Assuming your DTO looks like: public record UpdateUserRequest(UpdateUserDto user);
public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator(IConfiguration configuration)
    {
        var allowedExtensions = configuration.GetSection("FileUpload:AllowedExtensions").Get<string[]>() ?? Array.Empty<string>();
        var maxSizeBytes = configuration.GetValue<long>("FileUpload:MaxFileSizeBytes");

        When(x => x.user.Image != null && x.user.Image.Length > 0, () =>
        {
            RuleFor(x => x.user.Image)
                .Must(file => 
                {
                    var extension = Path.GetExtension(file!.FileName).ToLowerInvariant();
                    return allowedExtensions.Contains(extension);
                })
                .WithMessage("Invalid file type.");

            // Check Size
            RuleFor(x => x.user.Image)
                .Must(file => file!.Length <= maxSizeBytes)
                .WithMessage("File is too large.");
        });
    }
}