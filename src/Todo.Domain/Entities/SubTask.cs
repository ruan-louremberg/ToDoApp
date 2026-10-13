namespace ToDoApp.Domain.Entities;

public class SubTask : BaseEntity
{
    private SubTask()
    {
    }

    internal SubTask(ToDo task, string title, int position)
    {
        Task = task;
        TaskId = task.Id;
        Title = title;
        Position = position;
    }

    public Guid TaskId { get; private set; }

    public ToDo Task { get; private set; } = null!;

    public string Title { get; private set; } = null!;

    public bool IsCompleted { get; private set; }

    public int Position { get; private set; }

    internal void Rename(string title)
    {
        Title = title;
        SetUpdatedAt(DateTime.UtcNow);
    }

    internal void SetCompleted(bool isCompleted)
    {
        if (IsCompleted == isCompleted)
        {
            return;
        }

        IsCompleted = isCompleted;
        SetUpdatedAt(DateTime.UtcNow);
    }

    internal void SetPosition(int position)
    {
        if (Position == position)
        {
            return;
        }

        Position = position;
        SetUpdatedAt(DateTime.UtcNow);
    }
}
