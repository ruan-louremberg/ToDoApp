namespace ToDoApp.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = null!;

    public string Color { get; private set; } = null!;

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    public Category(string name, string color)
    {
        Name = name;
        Color = color;
        IsDeleted = false;
        DeletedAt = null;
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