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

    public async Task<ToDo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ToDos
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

   public async Task<List<ToDo>> GetAllAsync(
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
        var query = _context.ToDos.AsQueryable();

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

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.Title.Contains(search));
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
        Status? status,
        Priority? priority,
        Guid? categoryId,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ToDos.AsQueryable();

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

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.Title.Contains(search));
        }

        return await query.CountAsync(cancellationToken);
    }
    
    public async Task UpdateAsync(ToDo task, CancellationToken cancellationToken = default)
    {
        _context.ToDos.Update(task);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await GetByIdAsync(id, cancellationToken);
        if (task is not null)
        {
            _context.ToDos.Remove(task);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

