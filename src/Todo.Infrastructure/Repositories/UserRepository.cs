using Microsoft.EntityFrameworkCore;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;
using ToDoApp.Infrastructure.Persistence;

namespace ToDoApp.Infrastructure.Repositories;

public class UserRepository(TodoDbContext context) : IUserRepository
{
    private readonly TodoDbContext _context = context;

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return _context.Users
            .AnyAsync(user => user.Email == email, cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        if (_context.Entry(user).State == EntityState.Detached)
        {
            _context.Users.Update(user);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> HasValidPasswordResetTokenAsync(
        string tokenHash,
        DateTime now,
        CancellationToken cancellationToken = default)
    {
        return _context.Users.AnyAsync(
            user => user.PasswordResetTokenHash == tokenHash
                && user.PasswordResetTokenExpiresAt > now,
            cancellationToken);
    }

    public async Task<bool> ResetPasswordAsync(
        string tokenHash,
        string passwordHash,
        DateTime now,
        CancellationToken cancellationToken = default)
    {
        var affectedRows = await _context.Users
            .Where(user => user.PasswordResetTokenHash == tokenHash
                && user.PasswordResetTokenExpiresAt > now)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(user => user.PasswordHash, passwordHash)
                    .SetProperty(user => user.PasswordResetTokenHash, (string?)null)
                    .SetProperty(user => user.PasswordResetTokenExpiresAt, (DateTime?)null)
                    .SetProperty(user => user.UpdatedAt, now),
                cancellationToken);

        return affectedRows == 1;
    }
}
