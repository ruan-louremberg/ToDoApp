using Microsoft.EntityFrameworkCore;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Infrastructure.Persistence;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {
    }

    public DbSet<ToDo> ToDos => Set<ToDo>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ToDo>(builder =>
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Title).IsRequired().HasMaxLength(120);
            builder.Property(t => t.Description).HasMaxLength(1000);
            builder.Property(t => t.Status).HasConversion<string>();
            builder.Property(t => t.Priority).HasConversion<string>();
            builder.Property(t => t.DueDate);
            builder.Property(t => t.CompletedAt);
            builder.Property(t => t.CreatedAt).IsRequired();
            builder.Property(t => t.UpdatedAt);
            builder.Property(t => t.IsDeleted).IsRequired();
            builder.Property(t => t.DeletedAt);
            builder.Property(t => t.CategoryId);
            builder.HasOne(t => t.Category).WithMany().HasForeignKey(t => t.CategoryId);
            builder.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Category>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Color).IsRequired().HasMaxLength(50);
            builder.HasQueryFilter(c => !c.IsDeleted);
            builder.Property(c => c.IsDeleted).IsRequired();
            builder.Property(c => c.DeletedAt);
            builder.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(user => user.Id);
            builder.Property(user => user.Name)
                .IsRequired()
                .HasMaxLength(120);
            builder.Property(user => user.Email)
                .IsRequired()
                .HasMaxLength(320);
            builder.Property(user => user.PasswordHash)
                .IsRequired();
            builder.HasIndex(user => user.Email)
                .IsUnique();
        });
    }
}
