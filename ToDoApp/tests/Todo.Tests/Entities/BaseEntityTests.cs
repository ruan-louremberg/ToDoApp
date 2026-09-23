using ToDoApp.Domain.Entities;

namespace ToDoApp.Tests.Entities;

public class BaseEntityTests
{
    [Fact(DisplayName = "Inicializa identificador e data de criação")]
    public void GivenNewEntity_WhenConstructed_ThenInitializesIdentityAndCreationDate()
    {
        var entity = new TestEntity();

        Assert.NotEqual(Guid.Empty, entity.Id);
        Assert.InRange(entity.CreatedAt, DateTime.UtcNow.AddSeconds(-2), DateTime.UtcNow.AddSeconds(2));
        Assert.Null(entity.UpdatedAt);
    }

    [Fact(DisplayName = "Atualiza a data de alteração")]
    public void GivenEntity_WhenSettingUpdatedAt_ThenStoresDate()
    {
        var entity = new TestEntity();
        var updatedAt = DateTime.UtcNow.AddMinutes(1);

        entity.SetUpdatedAt(updatedAt);

        Assert.Equal(updatedAt, entity.UpdatedAt);
    }

    private sealed class TestEntity : BaseEntity;
}