using FluentValidation;
using ToDoApp.Application.DTO;

namespace ToDoApp.Application.Validators;

public class ListCategoriesRequestValidator : AbstractValidator<ListCategoriesRequest>
{
    public ListCategoriesRequestValidator()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("A página deve ser maior ou igual a 1.");

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("O tamanho da página deve estar entre 1 e 100.");
    }
}
