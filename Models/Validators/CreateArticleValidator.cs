using FluentValidation;
using RealWorld.Data;
using RealWorld.Models.DTOs.Articles;

namespace RealWorld.Models.Validators;

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