using ToDoApp.Application.DTO;
using ToDoApp.Application.Validators;
using ToDoApp.Domain;

namespace ToDoApp.Tests.Validators;

public class UpdateCategoryRequestValidatorTests
{
    [Fact(DisplayName = "Aceita campo explicitamente nulo em patch de categoria")]
    public void GivenExplicitNullField_WhenValidatingRequest_ThenReturnsSuccess()
    {
        var result = new UpdateCategoryRequestValidator().Validate(new UpdateCategoryRequest
        {
            Name = Optional<string>.Of(null),
            Color = Optional<string>.Of(null)
        });

        Assert.True(result.IsValid);
    }
}
