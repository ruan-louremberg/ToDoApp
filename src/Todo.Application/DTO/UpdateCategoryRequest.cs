namespace ToDoApp.Application.DTO;

public record UpdateCategoryRequest
{
    public string? Name { get; set; }

    public string? Color { get; set; }
}