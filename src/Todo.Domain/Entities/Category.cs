namespace ToDoApp.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = null!;

    public string Color { get; private set; } = null!;

    public Category(string name, string color)
    {
        Name = name;
        Color = color;
    }
}