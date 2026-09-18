using ToDoApp.Domain.Interfaces.Repositories;
using ToDoApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Infrastructure.Repositories;

public class SummaryRepository(TodoDbContext context) : ISummaryRepository
{
     private readonly TodoDbContext _context = context;

     public async Task<SummaryData> GetSummaryAsync(
          CancellationToken cancellationToken = default)
     {
          var now = DateTime.UtcNow;

          var summary = await _context.ToDos
               .AsNoTracking()
               .Where(task => !task.IsDeleted)
               .GroupBy(_ => 1)
               .Select(group => new SummaryData(
                    group.Count(),
                    group.Count(task => task.Status == Status.Pending),
                    group.Count(task => task.Status == Status.InProgress),
                    group.Count(task => task.Status == Status.Completed),
                    group.Count(task =>
                         task.DueDate.HasValue &&
                         task.DueDate.Value < now &&
                         task.Status != Status.Completed)))
               .SingleOrDefaultAsync(cancellationToken);

          return summary ?? new SummaryData(0, 0, 0, 0, 0);
     }

     public Task<int> CountAsync(CancellationToken cancellationToken = default)
     {
          return _context.ToDos
               .AsNoTracking()
               .CountAsync(task => !task.IsDeleted, cancellationToken);
     }
}