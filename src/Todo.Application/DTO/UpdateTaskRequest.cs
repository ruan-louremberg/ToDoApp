using ToDoApp.Domain;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.DTO;

public record UpdateTaskRequest
{
    public Optional<string> Title { get; set; } = Optional<string>.Unset();

    public Optional<string> Description { get; set; } = Optional<string>.Unset();

    public Optional<Priority?> Priority { get; set; } = Optional<Priority?>.Unset();

    public Optional<DateTime?> DueDate { get; set; } = Optional<DateTime?>.Unset();

    public Optional<Guid?> CategoryId { get; set; } = Optional<Guid?>.Unset();
}