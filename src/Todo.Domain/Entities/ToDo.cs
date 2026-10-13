using ToDoApp.Domain.Enums;
using ToDoApp.Domain.Exceptions;


namespace ToDoApp.Domain.Entities
{
    public class ToDo : BaseEntity
    {
        private readonly List<SubTask> _subtasks = [];

        public Guid UserId { get; private set; }

        public User User { get; private set; } = null!;

        public string Title { get; private set; } = null!;

        public string? Description { get; private set; }

        public Status Status { get; private set; }

        public Priority Priority { get; private set; }

        public DateTime? DueDate { get; private set; }

        public DateTime? CompletedAt { get; private set; }

        public Guid? CategoryId { get; private set; }

        public Category? Category { get; private set; }

        public bool IsDeleted { get; private set; }

        public DateTime? DeletedAt { get; private set; }

        public IReadOnlyCollection<SubTask> Subtasks => _subtasks.AsReadOnly();

        public ToDo(string title, string? description, Priority priority, DateTime? dueDate, Guid? categoryId = null)
            : this(Guid.Empty, title, description, priority, dueDate, categoryId)
        {
        }

        public ToDo(Guid userId, string title, string? description, Priority priority, DateTime? dueDate, Guid? categoryId = null)
        {
            UserId = userId;
            Title = title;
            Description = description;
            Status = Status.Pending;
            Priority = priority;
            DueDate = dueDate;

            CategoryId = categoryId;
        }
        public void SetCategory(Category category)
        {
            Category = category;
            CategoryId = category.Id;
        }

        public SubTask AddSubtask(string title)
        {
            var subtask = new SubTask(this, title, _subtasks.Count);
            _subtasks.Add(subtask);
            SetUpdatedAt(DateTime.UtcNow);
            return subtask;
        }

        public bool UpdateSubtask(Guid subtaskId, string? title, bool? isCompleted)
        {
            var subtask = _subtasks.FirstOrDefault(item => item.Id == subtaskId);
            if (subtask is null)
            {
                return false;
            }

            if (title is not null)
            {
                subtask.Rename(title);
            }

            if (isCompleted.HasValue)
            {
                subtask.SetCompleted(isCompleted.Value);
            }

            SetUpdatedAt(DateTime.UtcNow);
            return true;
        }

        public bool RemoveSubtask(Guid subtaskId)
        {
            var subtask = _subtasks.FirstOrDefault(item => item.Id == subtaskId);
            if (subtask is null)
            {
                return false;
            }

            _subtasks.Remove(subtask);
            for (var index = 0; index < _subtasks.Count; index++)
            {
                _subtasks[index].SetPosition(index);
            }

            SetUpdatedAt(DateTime.UtcNow);
            return true;
        }

        public bool ReorderSubtasks(IReadOnlyList<Guid> subtaskIds)
        {
            if (subtaskIds.Count != _subtasks.Count ||
                subtaskIds.Distinct().Count() != _subtasks.Count ||
                subtaskIds.Any(id => _subtasks.All(item => item.Id != id)))
            {
                return false;
            }

            var subtasksById = _subtasks.ToDictionary(item => item.Id);
            for (var index = 0; index < subtaskIds.Count; index++)
            {
                subtasksById[subtaskIds[index]].SetPosition(index);
            }

            SetUpdatedAt(DateTime.UtcNow);
            return true;
        }

        public void SetUpdate(string? title, Optional<string?> description, Priority? priority, Optional<DateTime?> dueDate, Optional<Guid?> categoryId)
        {
            Title = title ?? Title;
            Description = description.IsSet ? description.Value : Description;
            Priority = priority ?? Priority;
            DueDate = dueDate.IsSet ? dueDate.Value : DueDate;
            CategoryId = categoryId.IsSet ? categoryId.Value : CategoryId;

            SetUpdatedAt(DateTime.UtcNow);
        }

        public void ChangeStatus(Status newStatus)
        {
            if (Status == newStatus)
            {
                return;
            }

            var isAllowedTransition = (Status, newStatus) switch
            {
                (Status.Pending, Status.InProgress) => true,
                (Status.InProgress, Status.Pending or Status.Completed) => true,
                (Status.Completed, Status.InProgress) => true,
                _ => false,
            };

            if (!isAllowedTransition)
            {
                throw new DomainException(
                    $"Não é permitido alterar o status de '{Status}' para '{newStatus}'.");
            }

            Status = newStatus;
            CompletedAt = newStatus == Status.Completed ? DateTime.UtcNow : null;
            SetUpdatedAt(DateTime.UtcNow);
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        public void Restore()
        {
            IsDeleted = false;
            DeletedAt = null;
        }
    }
}