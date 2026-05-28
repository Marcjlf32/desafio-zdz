using Catalog.Application.Categorias;
using Catalog.Application.Common;
using Catalog.Application.Domain.Entities;
using Catalog.Application.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Produtos;

public class ProdutoService : IProdutoService
{
    private readonly ICatalogDbContext _db;
    private readonly IValidator<CreateProdutoDto> _createValidator;
    private readonly IValidator<UpdateProdutoDto> _updateValidator;

    public ProdutoService(
        ICatalogDbContext db,
        IValidator<CreateProdutoDto> createValidator,
        IValidator<UpdateProdutoDto> updateValidator)
    {
        _db = db;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<ProdutoDto>> ListAsync(CancellationToken cancellationToken)
    {
        return await _db.Produtos
            .AsNoTracking()
            .Include(p => p.Categoria)
            .OrderBy(p => p.Id)
            .Select(p => new ProdutoDto(
                p.Id,
                p.Nome,
                p.Descricao,
                p.Preco,
                p.CategoriaId,
                new CategoriaDto(p.Categoria.Id, p.Categoria.Nome, p.Categoria.Descricao)))
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<ProdutoDto>> CreateAsync(CreateProdutoDto dto, CancellationToken cancellationToken)
    {
        var validation = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<ProdutoDto>.ValidationFailed(ToErrorsDictionary(validation));
        }

        var categoria = await _db.Categorias.FirstOrDefaultAsync(c => c.Id == dto.CategoriaId, cancellationToken);
        if (categoria is null)
        {
            return Result<ProdutoDto>.ValidationFailed(new Dictionary<string, string[]>
            {
                ["categoriaId"] = new[] { "A categoria informada não existe." }
            });
        }

        var entity = new Produto
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            Preco = dto.Preco,
            CategoriaId = dto.CategoriaId
        };

        _db.Produtos.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<ProdutoDto>.Success(new ProdutoDto(
            entity.Id,
            entity.Nome,
            entity.Descricao,
            entity.Preco,
            entity.CategoriaId,
            new CategoriaDto(categoria.Id, categoria.Nome, categoria.Descricao)));
    }

    public async Task<Result> UpdateAsync(int id, UpdateProdutoDto dto, CancellationToken cancellationToken)
    {
        var validation = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.ValidationFailed(ToErrorsDictionary(validation));
        }

        var entity = await _db.Produtos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (entity is null)
        {
            return Result.NotFound();
        }

        var categoriaExists = await _db.Categorias.AnyAsync(c => c.Id == dto.CategoriaId, cancellationToken);
        if (!categoriaExists)
        {
            return Result.ValidationFailed(new Dictionary<string, string[]>
            {
                ["categoriaId"] = new[] { "A categoria informada não existe." }
            });
        }

        entity.Nome = dto.Nome;
        entity.Descricao = dto.Descricao;
        entity.Preco = dto.Preco;
        entity.CategoriaId = dto.CategoriaId;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _db.Produtos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (entity is null)
        {
            return Result.NotFound();
        }

        _db.Produtos.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static Dictionary<string, string[]> ToErrorsDictionary(FluentValidation.Results.ValidationResult validation)
    {
        return validation.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
    }
}
