using FluentValidation;
using RealWorld.Data;
using RealWorld.DTOs;
using Microsoft.EntityFrameworkCore;

namespace RealWorld.Validators;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator(AppDbContext context)
    {
        RuleFor(x => x.user.Username)
            .NotEmpty().WithMessage("can't be blank")
            .MinimumLength(3).WithMessage("must be at least 3 characters")
            // Custom Async Rule: Must be unique in the database
            .MustAsync(async (username, cancellation) => 
            {
                bool exists = await context.Users.AnyAsync(u => u.Username == username, cancellation);
                return !exists; // If it doesn't exist, it's valid!
            }).WithMessage("has already been taken");

        RuleFor(x => x.user.Email)
            .NotEmpty().WithMessage("can't be blank")
            .EmailAddress().WithMessage("is invalid")
            // Custom Async Rule: Must be unique
            .MustAsync(async (email, cancellation) => 
            {
                bool exists = await context.Users.AnyAsync(u => u.Email == email, cancellation);
                return !exists; 
            }).WithMessage("has already been taken");

        RuleFor(x => x.user.Password)
            .NotEmpty().WithMessage("can't be blank")
            .MinimumLength(8).WithMessage("is too short, must contain at least 8 characters");
    }
}