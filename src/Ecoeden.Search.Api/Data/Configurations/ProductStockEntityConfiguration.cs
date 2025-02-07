using Ecoeden.Search.Api.Entities.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecoeden.Search.Api.Data.Configurations;

public class ProductStockEntityConfiguration : IEntityTypeConfiguration<ProductStock>
{
    public void Configure(EntityTypeBuilder<ProductStock> builder)
    {
        builder.ToTable("ProductStocks", "ecoeden.stock");
        builder.Property(c => c.Id).ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEWID()");

        builder.Property(p => p.ProductId).IsRequired();
        builder.Property(p => p.SupplierId).IsRequired();
        builder.Property(p => p.Quantity).IsRequired();

        builder.HasKey(p => new { p.ProductId, p.SupplierId }).IsClustered();
    }
}
