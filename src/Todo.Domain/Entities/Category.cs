namespace ToDoApp.Domain.Entities;

public class Category : BaseEntity
{
    public Guid UserId { get; private set; }

    public User User { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public string Color { get; private set; } = null!;

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    public Category(string name, string color)
        : this(Guid.Empty, name, color)
    {
    }

    public Category(Guid userId, string name, string color)
    {
        UserId = userId;
        Name = name;
        Color = color;
        IsDeleted = false;
        DeletedAt = null;
    }

    public void Update(string name, string color)
    {
        Name = name;
        Color = color;
    }
    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
    }
}