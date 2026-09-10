using ToDoApp.Domain.Enums;
namespace ToDoApp.Application.DTO;

public record ListTasksResponse
{
    public List<TaskResponseList> Items { get; set; } = [];

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalItems { get; set; }

    public int TotalPages { get; set; }
}

public record TaskResponseList
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Status? Status { get; set; }

    public Priority? Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public Guid? CategoryId { get; set; }


}

public record ListTasksRequest
{
    public string? Search { get; set; } = null;

    public Status? Status { get; set; } = null;

    public Priority? Priority { get; set; } = null;

    public string? SortBy { get; set; } = "CreatedAt";

    public string? SortDirection { get; set; } = "desc";

    public Guid? CategoryId { get; set; } = null;

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}