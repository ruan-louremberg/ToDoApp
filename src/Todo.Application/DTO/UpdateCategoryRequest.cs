namespace ToDoApp.Application.DTO;

public record UpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;
}