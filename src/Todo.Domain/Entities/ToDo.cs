using ToDoApp.Domain.Enums;
using ToDoApp.Domain.Exceptions;


namespace ToDoApp.Domain.Entities
{
    public class ToDo : BaseEntity
    {

        public string Title { get; private set; } = null!;

        public string? Description { get; private set; }

        public Status Status { get; private set; }

        public Priority Priority { get; private set; }

        public DateTime? DueDate { get; private set; }

        public DateTime? CompletedAt { get; private set; }

        public Guid? CategoryId { get; private set; }

        public Category? Category { get; private set; }

        public ToDo(string title, string? description, Priority priority, DateTime? dueDate, Guid? categoryId = null)
        {
            if (title.Length < 3 || string.IsNullOrWhiteSpace(title))
            {
                throw new DomainException("O título da tarefa deve ter pelo menos 3 caracteres e não pode ser vazio.");
            }
            Title = title;
            Description = description;
            Status = Status.Pending;
            Priority = priority;
            DueDate = dueDate;
            
            CategoryId = categoryId;
        }

    }
}