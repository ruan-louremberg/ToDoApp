using ToDoApp.Domain.Entities;

namespace ToDoApp.Domain.Interfaces.Repositories;

public interface IToDoRepository
{
    Task AddAsync(ToDo task, CancellationToken cancellationToken = default);
    Task<ToDo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ToDo>> GetAllAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(ToDo task, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

