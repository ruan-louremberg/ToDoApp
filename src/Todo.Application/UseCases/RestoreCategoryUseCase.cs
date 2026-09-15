using FluentResults;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class RestoreCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public RestoreCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var restored = await _categoryRepository.RestoreAsync(
            id,
            cancellationToken);

        if (!restored)
        {
            return Result.Fail("Categoria não encontrada na lixeira.");
        }

        return Result.Ok();
    }
}