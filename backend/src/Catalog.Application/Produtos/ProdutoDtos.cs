using Catalog.Application.Categorias;

namespace Catalog.Application.Produtos;

public record ProdutoDto(int Id, string Nome, string? Descricao, decimal Preco, int CategoriaId, CategoriaDto Categoria);

public record CreateProdutoDto(string Nome, string? Descricao, decimal Preco, int CategoriaId);

public record UpdateProdutoDto(string Nome, string? Descricao, decimal Preco, int CategoriaId);
