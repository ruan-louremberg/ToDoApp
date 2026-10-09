using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Domain.Interfaces.Repositories;

public interface IToDoRepository
{
    Task AddAsync(ToDo task, CancellationToken cancellationToken = default);
    Task<ToDo?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
    Task<List<ToDo>> GetAllAsync(
        Guid userId,
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
        Guid userId,
        Status? status,
        Priority? priority,
        Guid? categoryId,
        string? search,
        CancellationToken cancellationToken = default);
    Task UpdateAsync(ToDo task, CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid userId,
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<ToDo>> GetTrashAsync(
        Guid userId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<int> CountTrashAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> RestoreAsync(
    Guid userId,
    Guid id,
    CancellationToken cancellationToken = default);
}
