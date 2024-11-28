using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductLib;

internal class ProductEntityTypeConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Code).IsUnique(true);

        builder.Property(x => x.Id)
            .IsRequired(true)
            .HasColumnName(nameof(Product.Id))
            .HasColumnType("varchar")
            .HasMaxLength(36)
            .IsUnicode(false)
            ;
        builder.Property(x => x.Code)
            .IsRequired(true)
            .HasColumnName(nameof(Product.Code))
            .HasColumnType("nvarchar")
            .HasMaxLength(20)
            .IsUnicode(true)
            ;
        builder.Property(x => x.Name)
            .IsRequired(false)
            .HasColumnName(nameof(Product.Name))
            .HasColumnType("nvarchar")
            .HasMaxLength(50)
            .IsUnicode(true)
            ;
        builder.Property(x => x.Category)
            .IsRequired(true)
            .HasColumnName(nameof(Product.Category))
            .HasColumnType("tinyint")
            ;
        builder.Property(x => x.Created)
            .IsRequired(false)
            .HasColumnName(nameof(Product.Created))
            .HasColumnType("datetime")
            ;
        builder.Property(x => x.LastUpdated)
            .IsRequired(false)
            .HasColumnName(nameof(Product.LastUpdated))
            .HasColumnType("datetime")
            ;
    }
}