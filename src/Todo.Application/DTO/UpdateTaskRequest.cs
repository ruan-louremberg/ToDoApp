using System.ComponentModel.DataAnnotations;
using ToDoApp.Application.Common;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.DTO;

public record UpdateTaskRequest
{
    public Option<string> Title { get; init; }
    public Option<string> Description { get; init; }
    
    [EnumDataType(typeof(Priority))]
    public Option<Priority> Priority { get; init; }
    
    public Option<DateTime?> DueDate { get; init; }
    public Option<Guid?> CategoryId { get; init; }
}