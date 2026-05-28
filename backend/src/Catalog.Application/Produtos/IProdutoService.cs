using Catalog.Application.Common;

namespace Catalog.Application.Produtos;

public interface IProdutoService
{
    Task<IReadOnlyList<ProdutoDto>> ListAsync(CancellationToken cancellationToken);
    Task<Result<ProdutoDto>> CreateAsync(CreateProdutoDto dto, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(int id, UpdateProdutoDto dto, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);
}
