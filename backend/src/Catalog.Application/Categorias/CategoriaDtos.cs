namespace Catalog.Application.Categorias;

public record CategoriaDto(int Id, string Nome, string? Descricao);

public record CreateCategoriaDto(string Nome, string? Descricao);

public record UpdateCategoriaDto(string Nome, string? Descricao);
