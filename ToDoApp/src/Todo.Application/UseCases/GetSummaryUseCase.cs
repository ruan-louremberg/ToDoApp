using ToDoApp.Application.DTO;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class GetSummaryUseCase
{
    private readonly ISummaryRepository _summaryRepository;

    public GetSummaryUseCase(ISummaryRepository summaryRepository)
    {
        _summaryRepository = summaryRepository;
    }

    public async Task<TaskSummaryResponse> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var summary = await _summaryRepository.GetSummaryAsync(cancellationToken);

        return new TaskSummaryResponse
        {
            TotalTasks = summary.TotalTasks,
            Status = new StatusBreakdown(
                summary.Pending,
                summary.InProgress,
                summary.Completed),
            Delays = new DelayBreakdown(summary.Overdue)
        };
    }
}