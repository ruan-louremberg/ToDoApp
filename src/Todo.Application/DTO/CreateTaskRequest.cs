using System.ComponentModel.DataAnnotations;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.DTO;

public record CreateTaskRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [EnumDataType(typeof(Priority))]
    public Priority Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public Guid? CategoryId { get; set; }
}
