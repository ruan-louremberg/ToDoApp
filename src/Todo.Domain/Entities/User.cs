namespace ToDoApp.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public string PasswordHash { get; private set; } = null!;

    public string? PasswordResetTokenHash { get; private set; }

    public DateTime? PasswordResetTokenExpiresAt { get; private set; }

    public DateTime? PasswordResetRequestedAt { get; private set; }

    public User(string name, string email, string passwordHash)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        PasswordResetTokenHash = null;
        PasswordResetTokenExpiresAt = null;
        SetUpdatedAt(DateTime.UtcNow);
    }

    public void SetPasswordResetToken(
        string tokenHash,
        DateTime expiresAt,
        DateTime requestedAt)
    {
        PasswordResetTokenHash = tokenHash;
        PasswordResetTokenExpiresAt = expiresAt;
        PasswordResetRequestedAt = requestedAt;
        SetUpdatedAt(requestedAt);
    }
}