using ToDoApp.Application.DTO;
using ToDoApp.Application.Validators;

namespace ToDoApp.Tests.Validators;

public class SubTaskRequestValidatorTests
{
    [Theory(DisplayName = "Rejeita título vazio ou maior que 120 caracteres")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")]
    public void GivenInvalidTitle_WhenValidatingCreateRequest_ThenReturnsValidationError(string title)
    {
        var result = new CreateSubTaskRequestValidator().Validate(new CreateSubTaskRequest { Title = title });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateSubTaskRequest.Title));
    }

    [Fact(DisplayName = "Aceita título válido e remove espaços externos no caso de uso")]
    public void GivenValidTitle_WhenValidatingCreateRequest_ThenReturnsSuccess()
    {
        var result = new CreateSubTaskRequestValidator()
            .Validate(new CreateSubTaskRequest { Title = "  Buy milk  " });

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Rejeita atualização sem campos")]
    public void GivenEmptyUpdate_WhenValidatingRequest_ThenReturnsValidationError()
    {
        var result = new UpdateSubTaskRequestValidator().Validate(new UpdateSubTaskRequest());

        Assert.False(result.IsValid);
    }

    [Fact(DisplayName = "Rejeita ordenação com identificadores duplicados")]
    public void GivenDuplicateIdentifiers_WhenValidatingOrder_ThenReturnsValidationError()
    {
        var id = Guid.NewGuid();
        var result = new ReorderSubTasksRequestValidator()
            .Validate(new ReorderSubTasksRequest { SubtaskIds = [id, id] });

        Assert.False(result.IsValid);
    }
}
