using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Domain.Interfaces.Repositories;

public interface IToDoRepository
{
    Task AddAsync(ToDo task, CancellationToken cancellationToken = default);
    Task<ToDo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ToDo>> GetAllAsync(
        Status? status,
        Priority? priority,
        Guid? categoryId,
        string? search,
        string? sortBy,
        string? sortDirection,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Status? status,
        Priority? priority,
        Guid? categoryId,
        string? search,
        CancellationToken cancellationToken = default);
    Task UpdateAsync(ToDo task, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
