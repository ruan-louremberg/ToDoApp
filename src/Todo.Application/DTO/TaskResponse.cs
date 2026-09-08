namespace ToDoApp.Application.DTO;

public record CategoryResponse(
    Guid Id,
    string Name,
    string Color
);
public record TaskResponse(
    Guid Id,
    string Title,
    string? Description,
    int Priority,
    DateTime? DueDate,
    CategoryResponse? Category
);