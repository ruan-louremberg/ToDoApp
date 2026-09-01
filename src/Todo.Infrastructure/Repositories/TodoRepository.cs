using Microsoft.EntityFrameworkCore;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;
using ToDoApp.Infrastructure.Persistence;

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

    public async Task<IEnumerable<ToDo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ToDos
            .AsNoTracking()
            .ToListAsync(cancellationToken);
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

