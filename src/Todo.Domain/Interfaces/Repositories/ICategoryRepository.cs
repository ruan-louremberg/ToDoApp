using ToDoApp.Domain.Entities;

namespace ToDoApp.Domain.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task AddAsync(Category category, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);

    Task<Category?> GetByIdAsync(Guid userId, Guid value, CancellationToken cancellationToken);

    Task<Category?> GetByNameAsync(Guid userId, string name, CancellationToken cancellationToken);

    Task<List<Category>> GetAllAsync(
        Guid userId,
        string? name,
        string? color,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid userId,
        string? name,
        string? color,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(Category category, CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid userId,
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<Category>> GetTrashAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> RestoreAsync(
    Guid userId,
    Guid id,
    CancellationToken cancellationToken = default);

    Task<Category?> GetDeletedByIdAsync(
    Guid userId,
    Guid id,
    CancellationToken cancellationToken = default);
}