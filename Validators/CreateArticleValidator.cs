using FluentValidation;
using RealWorld.Data;
using RealWorld.DTOs;
using Microsoft.EntityFrameworkCore;

namespace RealWorld.Validators;

public class CreateArticleValidator : AbstractValidator<CreateArticleRequest>
{
    public CreateArticleValidator(AppDbContext context)
    {
        RuleFor(x => x.article.Title)
            .NotEmpty().WithMessage("can't be blank");
        
        RuleFor(x => x.article.Description)
            .NotEmpty().WithMessage("can't be blank");

        RuleFor(x => x.article.Body)
            .NotEmpty().WithMessage("can't be blank");
    }
}