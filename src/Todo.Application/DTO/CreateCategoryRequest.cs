namespace ToDoApp.Application.DTO;

public record CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;


}
