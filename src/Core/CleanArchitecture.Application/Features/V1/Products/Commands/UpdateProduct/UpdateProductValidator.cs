using FluentValidation;

namespace CleanArchitecture.Application.Features.V1.Products.Commands.UpdateProduct;

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty();

        RuleFor(p => p.Name)
            .NotEmpty();

        RuleFor(p => p.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(p => p.CategoryId)
            .NotEmpty();
    }
}
