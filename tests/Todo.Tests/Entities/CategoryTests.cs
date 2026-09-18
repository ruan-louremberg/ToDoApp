using ToDoApp.Domain.Entities;

namespace ToDoApp.Tests.Entities;

public class CategoryTests
{
    [Fact(DisplayName = "Cria categoria ativa sem data de exclusão")]
    public void GivenCategoryData_WhenConstructed_ThenInitializesActiveCategory()
    {
        var category = new Category("Work", "#FFFFFF");

        Assert.Equal("Work", category.Name);
        Assert.Equal("#FFFFFF", category.Color);
        Assert.False(category.IsDeleted);
        Assert.Null(category.DeletedAt);
    }

    [Fact(DisplayName = "Marca categoria como excluída logicamente")]
    public void GivenActiveCategory_WhenSoftDeleted_ThenSetsDeletionState()
    {
        var category = new Category("Work", "#FFFFFF");

        category.SoftDelete();

        Assert.True(category.IsDeleted);
        Assert.NotNull(category.DeletedAt);
    }

    [Fact(DisplayName = "Restaura categoria excluída")]
    public void GivenDeletedCategory_WhenRestored_ThenClearsDeletionState()
    {
        var category = new Category("Work", "#FFFFFF");
        category.SoftDelete();

        category.Restore();

        Assert.False(category.IsDeleted);
        Assert.Null(category.DeletedAt);
    }
}