using FluentValidation;
using ToDoApp.Application.DTO;

namespace ToDoApp.Application.Validators;

public class ListTasksRequestValidator : AbstractValidator<ListTasksRequest>
{
    private static readonly string[] AllowedSortBy =
    [
        "CreatedAt",
        "DueDate",
        "Priority"
    ];

    private static readonly string[] AllowedSortDirections =
    [
        "asc",
        "desc"
    ];

    public ListTasksRequestValidator()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("A página deve ser maior ou igual a 1.");

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("O tamanho da página deve estar entre 1 e 100.");

        RuleFor(request => request.SortBy)
            .Must(sortBy => AllowedSortBy.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
            .WithMessage("O campo de ordenação é inválido.");

        RuleFor(request => request.SortDirection)
            .Must(direction => AllowedSortDirections.Contains(direction, StringComparer.OrdinalIgnoreCase))
            .WithMessage("A direção da ordenação deve ser asc ou desc.");
    }
}
