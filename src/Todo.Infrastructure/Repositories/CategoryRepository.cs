using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;
using ToDoApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ToDoApp.Infrastructure.Repositories;

public class CategoryRepository(TodoDbContext context) : ICategoryRepository
{

    private readonly TodoDbContext _context = context;

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        await _context.Categories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories.AnyAsync(c => c.Id == id, cancellationToken);
    }
    public async Task<Category?> GetByIdAsync(Guid value, CancellationToken cancellationToken)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == value, cancellationToken);
    }
    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }
    
}