namespace ToDoApp.Application.DTO;

public record CreateCategoryRequest
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;


}
