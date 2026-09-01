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
            builder.Property(t => t.CategoryId);
            builder.HasOne(t => t.Category).WithMany().HasForeignKey(t => t.CategoryId);
        });

        modelBuilder.Entity<Category>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Color).IsRequired().HasMaxLength(50);
        });
    }
}

