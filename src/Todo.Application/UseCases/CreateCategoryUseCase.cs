using FluentResults;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class CreateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<Category>> ExecuteAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = new Category(request.Name, request.Color);
        var existingCategory = await _categoryRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingCategory != null)
        {
            return Result.Fail("Categoria já existe.");
        }
        await _categoryRepository.AddAsync(category, cancellationToken);
        return category;
    }
}