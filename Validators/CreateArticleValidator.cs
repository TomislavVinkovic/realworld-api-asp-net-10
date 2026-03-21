using FluentValidation;
using dotnet_api_tutorial.Data;
using dotnet_api_tutorial.DTOs;
using Microsoft.EntityFrameworkCore;

namespace dotnet_api_tutorial.Validators;

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