using ToDoApp.Application.DTO;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class GetTrashCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public GetTrashCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<CategoryResponse>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetTrashAsync(cancellationToken);

        return categories.Select(category => new CategoryResponse(
            category.Id,
            category.Name,
            category.Color
        )).ToList();
        
    }
}