using System.Security.Cryptography.X509Certificates;
using ToDoApp.Domain.Enums;
namespace ToDoApp.Application.DTO;

public record CompleteTaskRequest
{

    public Status Status { get; set; }
};

public class CompleteTaskResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Status Status { get; set; }

    public Priority Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public Guid? CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }
}