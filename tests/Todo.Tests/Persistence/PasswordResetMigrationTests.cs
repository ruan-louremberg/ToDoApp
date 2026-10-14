using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Domain.Entities;
using ToDoApp.Infrastructure.Persistence;
using ToDoApp.Infrastructure.Repositories;

namespace ToDoApp.Tests.Persistence;

public class PasswordResetMigrationTests
{
    [Fact]
    public async Task GivenFreshDatabase_WhenApplyingMigrations_ThenPasswordResetTokensArePersistedAndConsumedOnce()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseSqlite(connection)
            .Options;
        await using var context = new TodoDbContext(options);

        await context.Database.MigrateAsync();

        var user = new User("Maria", "maria@example.com", "old-password-hash");
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        const string tokenHash = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
        var now = DateTime.UtcNow;
        user.SetPasswordResetToken(tokenHash, now.AddMinutes(15));
        var repository = new UserRepository(context);
        await repository.UpdateAsync(user);

        Assert.True(await repository.HasValidPasswordResetTokenAsync(tokenHash, now));
        Assert.True(await repository.ResetPasswordAsync(tokenHash, "new-password-hash", now));
        Assert.False(await repository.HasValidPasswordResetTokenAsync(tokenHash, now));
        Assert.False(await repository.ResetPasswordAsync(tokenHash, "another-password-hash", now));

        context.ChangeTracker.Clear();
        var updatedUser = await context.Users.SingleAsync(item => item.Id == user.Id);
        Assert.Equal("new-password-hash", updatedUser.PasswordHash);
        Assert.Null(updatedUser.PasswordResetTokenHash);
        Assert.Null(updatedUser.PasswordResetTokenExpiresAt);
    }
}
