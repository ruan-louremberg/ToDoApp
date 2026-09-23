namespace ToDoApp.Application.DTO;

public record TaskSummaryResponse
{
    public StatusBreakdown Status { get; init; } = default!;
    public DelayBreakdown Delays { get; init; } = default!;
    public int TotalTasks { get; init; }
}

public record StatusBreakdown(
    int Pending,
    int InProgress,
    int Completed
);

public record DelayBreakdown(
    int Overdue
);