using Microsoft.EntityFrameworkCore;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;
using ToDoApp.Infrastructure.Persistence;

namespace ToDoApp.Infrastructure.Repositories;

public class CategoryRepository(TodoDbContext context) : ICategoryRepository
{
    private readonly TodoDbContext _context = context;

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        await _context.Categories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid userId, Guid id, CancellationToken cancellationToken = default) =>
        _context.Categories.AnyAsync(c => c.UserId == userId && c.Id == id, cancellationToken);

    public Task<Category?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken) =>
        _context.Categories.FirstOrDefaultAsync(c => c.UserId == userId && c.Id == id, cancellationToken);

    public Task<Category?> GetByNameAsync(Guid userId, string name, CancellationToken cancellationToken) =>
        _context.Categories.FirstOrDefaultAsync(c => c.UserId == userId && c.Name == name, cancellationToken);

    public async Task<List<Category>> GetAllAsync(
        Guid userId,
        string? name,
        string? color,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .AsQueryable();
        var safeName = name?.Trim();

        if (!string.IsNullOrWhiteSpace(safeName))
        {
            query = query.Where(c => c.Name.ToLower().Contains(safeName.ToLower()));
        }

        if (!string.IsNullOrEmpty(color))
        {
            query = query.Where(c => c.Color.Contains(color));
        }

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid userId,
        string? name,
        string? color,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .AsQueryable();
        var safeName = name?.Trim();

        if (!string.IsNullOrWhiteSpace(safeName))
        {
            query = query.Where(c => c.Name.ToLower().Contains(safeName.ToLower()));
        }

        if (!string.IsNullOrEmpty(color))
        {
            query = query.Where(c => c.Color.Contains(color));
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == id && !c.IsDeleted, cancellationToken);

        if (category is not null)
        {
            category.SoftDelete();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public Task<List<Category>> GetTrashAsync(Guid userId, CancellationToken cancellationToken = default) =>
        _context.Categories
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(c => c.UserId == userId && c.IsDeleted)
            .OrderByDescending(c => c.DeletedAt)
            .ToListAsync(cancellationToken);

    public async Task<bool> RestoreAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == id && c.IsDeleted, cancellationToken);

        if (category is null)
        {
            return false;
        }

        category.Restore();
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<Category?> GetDeletedByIdAsync(
        Guid userId,
        Guid id,
        CancellationToken cancellationToken = default) =>
        _context.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == id && c.IsDeleted, cancellationToken);
}
