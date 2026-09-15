using FluentResults;
using ToDoApp.Domain.Interfaces.Repositories;
namespace ToDoApp.Application.UseCases;
public class DeleteCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;
    public DeleteCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<Result> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
        {
            return Result.Fail("Categoria não encontrada.");
        }
        category.SoftDelete();
        await _categoryRepository.UpdateAsync(category, cancellationToken);
        return Result.Ok();
    }
}