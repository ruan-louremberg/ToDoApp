namespace ToDoApp.Application.DTO;

public record ListTasksRequest
{
    public string? Search { get; set; }

    public Status? Status { get; set; }

    public Priority? Priority { get; set; }

    public String? SortBy { get; set; }

    public String? SortDirection { get; set; }

    public Guid? CategoryId { get; set; }

    public int? Page { get; set; } = 1;

    public int? PageSize { get; set; } = 20;
}