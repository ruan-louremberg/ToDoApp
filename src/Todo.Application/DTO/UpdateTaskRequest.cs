using System.ComponentModel.DataAnnotations;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.DTO;

public record UpdateTaskRequest
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    [EnumDataType(typeof(Priority))]
    public Priority? Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public Guid? CategoryId { get; set; }
}