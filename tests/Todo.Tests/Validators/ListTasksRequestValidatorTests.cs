using ToDoApp.Application.DTO;
using ToDoApp.Application.Validators;

namespace ToDoApp.Tests.Validators;

public class ListTasksRequestValidatorTests
{
    [Fact(DisplayName = "Aceita filtros e ordenação válidos")]
    public void GivenValidListRequest_WhenValidatingRequest_ThenReturnsSuccess()
    {
        var result = new ListTasksRequestValidator().Validate(new ListTasksRequest
        {
            Page = 1,
            PageSize = 100,
            SortBy = "duedate",
            SortDirection = "ASC"
        });

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Rejeita paginação e ordenação inválidas")]
    public void GivenInvalidListRequest_WhenValidatingRequest_ThenReturnsValidationErrors()
    {
        var result = new ListTasksRequestValidator().Validate(new ListTasksRequest
        {
            Page = 0,
            PageSize = 101,
            SortBy = "Title",
            SortDirection = "random"
        });

        Assert.False(result.IsValid);
        Assert.Equal(4, result.Errors.Count);
    }
}