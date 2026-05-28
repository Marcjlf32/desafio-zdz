using Catalog.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("produtos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Nome)
            .HasColumnName("nome")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(1000);

        builder.Property(p => p.Preco)
            .HasColumnName("preco")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.CategoriaId)
            .HasColumnName("categoria_id")
            .IsRequired();
    }
}
