using ToDoApp.Domain.Entities;

namespace ToDoApp.Domain.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task AddAsync(Category category, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Category?> GetByIdAsync(Guid value, CancellationToken cancellationToken);

    Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken);

    Task<List<Category>> GetAllAsync(
        string? name,
        string? color,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        string? name,
        string? color,
        CancellationToken cancellationToken = default);
}