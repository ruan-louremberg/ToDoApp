using ToDoApp.Domain.Entities;

namespace ToDoApp.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    Task<bool> TrySchedulePasswordResetAsync(
        string email,
        string tokenHash,
        DateTime tokenExpiresAt,
        DateTime requestedAt,
        DateTime cooldownCutoff,
        CancellationToken cancellationToken = default);

    Task<bool> HasValidPasswordResetTokenAsync(
        string tokenHash,
        DateTime now,
        CancellationToken cancellationToken = default);

    Task<string?> GetPasswordHashForValidPasswordResetTokenAsync(
        string tokenHash,
        DateTime now,
        CancellationToken cancellationToken = default);

    Task<bool> ResetPasswordAsync(
        string tokenHash,
        string passwordHash,
        DateTime now,
        CancellationToken cancellationToken = default);
}
