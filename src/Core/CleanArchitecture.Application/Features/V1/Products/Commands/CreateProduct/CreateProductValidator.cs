using FluentValidation;

namespace CleanArchitecture.Application.Features.V1.Products.Commands.CreateProduct;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty();

        RuleFor(p => p.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(p => p.CategoryId)
            .NotEmpty();
    }
}
