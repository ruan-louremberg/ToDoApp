using ToDoApp.Application.DTO;
using ToDoApp.Application.Validators;

namespace ToDoApp.Tests.Validators;

public class ListCategoriesRequestValidatorTests
{
    [Fact(DisplayName = "Aceita paginação válida de categorias")]
    public void GivenValidListRequest_WhenValidatingRequest_ThenReturnsSuccess()
    {
        var result = new ListCategoriesRequestValidator().Validate(new ListCategoriesRequest { Page = 1, PageSize = 100 });

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Rejeita paginação inválida de categorias")]
    public void GivenInvalidListRequest_WhenValidatingRequest_ThenReturnsValidationErrors()
    {
        var result = new ListCategoriesRequestValidator().Validate(new ListCategoriesRequest { Page = 0, PageSize = 101 });

        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
    }
}