using Microsoft.EntityFrameworkCore;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;
using ToDoApp.Infrastructure.Persistence;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Infrastructure.Repositories;

public class TodoRepository : IToDoRepository
{
    private readonly TodoDbContext _context;

    public TodoRepository(TodoDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ToDo task, CancellationToken cancellationToken = default)
    {
        await _context.ToDos.AddAsync(task, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ToDo?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ToDos
            .Include(t => t.Category)
            .FirstOrDefaultAsync(
                t => t.UserId == userId && t.Id == id && !t.IsDeleted,
                cancellationToken);
    }

   public async Task<List<ToDo>> GetAllAsync(
        Guid userId,
        Status? status,
        Priority? priority,
        Guid? categoryId,
        string? search,
        string? sortBy,
        string? sortDirection,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ToDos
            .Include(t => t.Category)
            .AsNoTracking()
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(x => x.Priority == priority.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == categoryId.Value);
        }

        var safeSearch = search?.Trim();

        if (!string.IsNullOrWhiteSpace(safeSearch))
        {
            query = query.Where(x =>
                x.Title.ToLower().Contains(safeSearch.ToLower()));
        }

        var isDescending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        if (string.Equals(sortBy, "dueDate", StringComparison.OrdinalIgnoreCase))
        {
            query = isDescending
                ? query.OrderByDescending(x => x.DueDate)
                : query.OrderBy(x => x.DueDate);
        }
        else if (string.Equals(sortBy, "priority", StringComparison.OrdinalIgnoreCase))
        {
            query = isDescending
                ? query.OrderByDescending(x => x.Priority == Priority.High ? 3 : x.Priority == Priority.Medium ? 2 : 1)
                : query.OrderBy(x => x.Priority == Priority.High ? 3 : x.Priority == Priority.Medium ? 2 : 1);
        }
        else
        {
            query = isDescending
                ? query.OrderByDescending(x => x.CreatedAt)
                : query.OrderBy(x => x.CreatedAt);
        }

        var skip = (page - 1) * pageSize;

        return await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid userId,
        Status? status,
        Priority? priority,
        Guid? categoryId,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ToDos
            .AsNoTracking()
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(x => x.Priority == priority.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == categoryId.Value);
        }

        var safeSearch = search?.Trim();

        if (!string.IsNullOrWhiteSpace(safeSearch))
        {
            query = query.Where(x =>
                x.Title.ToLower().Contains(safeSearch.ToLower()));
        }

        return await query.CountAsync(cancellationToken);
    }
    
    public async Task UpdateAsync(ToDo task, CancellationToken cancellationToken = default)
    {
        _context.ToDos.Update(task);
        await _context.SaveChangesAsync(cancellationToken);
    }

     public async Task DeleteAsync(
        Guid userId,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var task = await _context.ToDos
            .FirstOrDefaultAsync(
                t => t.UserId == userId && t.Id == id && !t.IsDeleted,
                cancellationToken);

        if (task is not null)
        {
            task.SoftDelete();

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<List<ToDo>> GetTrashAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _context.ToDos
            .Include(t => t.Category)
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.IsDeleted)
            .OrderByDescending(t => t.DeletedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountTrashAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ToDos
            .Where(t => t.UserId == userId && t.IsDeleted)
            .CountAsync(cancellationToken);
    }

    public async Task<bool> RestoreAsync(
    Guid userId,
    Guid id,
    CancellationToken cancellationToken = default)
{
    var task = await _context.ToDos
        .FirstOrDefaultAsync(
            t => t.UserId == userId && t.Id == id && t.IsDeleted,
            cancellationToken);

    if (task is null)
    {
        return false;
    }

    task.Restore();

    await _context.SaveChangesAsync(cancellationToken);

    return true;
}
}
