using Catalog.Application.Common;

namespace Catalog.Application.Categorias;

public interface ICategoriaService
{
    Task<IReadOnlyList<CategoriaDto>> ListAsync(CancellationToken cancellationToken);
    Task<Result<CategoriaDto>> CreateAsync(CreateCategoriaDto dto, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(int id, UpdateCategoriaDto dto, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);
}
