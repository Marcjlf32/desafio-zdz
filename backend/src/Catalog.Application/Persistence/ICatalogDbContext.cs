using Catalog.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Persistence;

public interface ICatalogDbContext
{
    DbSet<Categoria> Categorias { get; }
    DbSet<Produto> Produtos { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
