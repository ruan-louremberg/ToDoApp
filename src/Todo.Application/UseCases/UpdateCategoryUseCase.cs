using FluentResults;
using FluentValidation;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class UpdateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<UpdateCategoryRequest> _validator;

    public UpdateCategoryUseCase(
        ICategoryRepository categoryRepository,
        IValidator<UpdateCategoryRequest> validator)
    {
        _categoryRepository = categoryRepository;
        _validator = validator;
    }

    public async Task<Result<CategoryResponse>> ExecuteAsync(
        Guid id,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.Select(error =>
                new Error(error.ErrorMessage).WithMetadata("statusCode", 400)));
        }

        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
        {
            return Result.Fail(new Error("Categoria não encontrada.")
                .WithMetadata("statusCode", 404));
        }

        var categoryWithSameName = await _categoryRepository.GetByNameAsync(request.Name, cancellationToken);
        if (categoryWithSameName is not null && categoryWithSameName.Id != category.Id)
        {
            return Result.Fail(new Error("Já existe uma categoria com esse nome.")
                .WithMetadata("statusCode", 409));
        }

        category.Update(request.Name, request.Color);
        await _categoryRepository.UpdateAsync(category, cancellationToken);

        return Result.Ok(new CategoryResponse(category.Id, category.Name, category.Color));
    }
}