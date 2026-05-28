using Catalog.Application.Common;
using Catalog.Application.Domain.Entities;
using Catalog.Application.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Categorias;

public class CategoriaService : ICategoriaService
{
    private readonly ICatalogDbContext _db;
    private readonly IValidator<CreateCategoriaDto> _createValidator;
    private readonly IValidator<UpdateCategoriaDto> _updateValidator;

    public CategoriaService(
        ICatalogDbContext db,
        IValidator<CreateCategoriaDto> createValidator,
        IValidator<UpdateCategoriaDto> updateValidator)
    {
        _db = db;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<CategoriaDto>> ListAsync(CancellationToken cancellationToken)
    {
        return await _db.Categorias
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Select(c => new CategoriaDto(c.Id, c.Nome, c.Descricao))
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<CategoriaDto>> CreateAsync(CreateCategoriaDto dto, CancellationToken cancellationToken)
    {
        var validation = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<CategoriaDto>.ValidationFailed(ToErrorsDictionary(validation));
        }

        var entity = new Categoria
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao
        };

        _db.Categorias.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<CategoriaDto>.Success(new CategoriaDto(entity.Id, entity.Nome, entity.Descricao));
    }

    public async Task<Result> UpdateAsync(int id, UpdateCategoriaDto dto, CancellationToken cancellationToken)
    {
        var validation = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.ValidationFailed(ToErrorsDictionary(validation));
        }

        var entity = await _db.Categorias.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (entity is null)
        {
            return Result.NotFound();
        }

        entity.Nome = dto.Nome;
        entity.Descricao = dto.Descricao;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _db.Categorias.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (entity is null)
        {
            return Result.NotFound();
        }

        var hasProdutos = await _db.Produtos.AnyAsync(p => p.CategoriaId == id, cancellationToken);
        if (hasProdutos)
        {
            return Result.Conflict("Não é possível excluir uma categoria que possua produtos vinculados.");
        }

        _db.Categorias.Remove(entity);
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
