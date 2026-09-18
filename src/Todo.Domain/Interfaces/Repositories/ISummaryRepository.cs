namespace ToDoApp.Domain.Interfaces.Repositories;

public interface ISummaryRepository
{
    Task<SummaryData> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}

public record SummaryData(
    int TotalTasks,
    int Pending,
    int InProgress,
    int Completed,
    int Overdue
);