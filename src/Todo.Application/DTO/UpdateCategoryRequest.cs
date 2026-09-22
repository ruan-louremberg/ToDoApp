using ToDoApp.Domain;

namespace ToDoApp.Application.DTO;

public record UpdateCategoryRequest
{
    public Optional<string> Name { get; set; } = Optional<string>.Unset();

    public Optional<string> Color { get; set; } = Optional<string>.Unset();
}