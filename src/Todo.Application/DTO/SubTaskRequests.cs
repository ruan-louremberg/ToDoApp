namespace ToDoApp.Application.DTO;

public sealed record CreateSubTaskRequest
{
    public string Title { get; init; } = string.Empty;
}

public sealed record UpdateSubTaskRequest
{
    public string? Title { get; init; }

    public bool? IsCompleted { get; init; }
}

public sealed record ReorderSubTasksRequest
{
    public List<Guid>? SubtaskIds { get; init; } = [];
}
