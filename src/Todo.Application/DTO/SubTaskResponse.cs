using ToDoApp.Domain.Entities;

namespace ToDoApp.Application.DTO;

public sealed record SubTaskResponse(
    Guid Id,
    string Title,
    bool IsCompleted,
    int Position,
    DateTime CreatedAt,
    DateTime? UpdatedAt)
{
    public static SubTaskResponse FromEntity(SubTask subtask) =>
        new(
            subtask.Id,
            subtask.Title,
            subtask.IsCompleted,
            subtask.Position,
            subtask.CreatedAt,
            subtask.UpdatedAt);
}
