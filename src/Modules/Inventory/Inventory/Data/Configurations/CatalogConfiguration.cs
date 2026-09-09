using Inventory.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Data.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(product => product.Id);
        builder.Property(product => product.Id).ValueGeneratedNever();

        builder.Property(product => product.Name).HasMaxLength(200).IsRequired();
        builder.Property(product => product.Description).HasMaxLength(2000);

        // WithOne() takes no argument on purpose: a variant has no navigation back to its
        // product. Restrict, not the default cascade, so deleting a product that still has
        // variants is refused by the database instead of taking their SKUs down with it.
        builder.HasMany(product => product.Variants)
            .WithOne()
            .HasForeignKey(variant => variant.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class VariantConfiguration : IEntityTypeConfiguration<Variant>
{
    public void Configure(EntityTypeBuilder<Variant> builder)
    {
        builder.ToTable("Variants");
        builder.HasKey(variant => variant.Id);

        // The domain hands out the Id, the database never does. Without this, EF assumes a key
        // that already has a value came from the store, so a variant added to a loaded product
        // is saved as an UPDATE of a row that does not exist yet.
        builder.Property(variant => variant.Id).ValueGeneratedNever();

        builder.Property(variant => variant.Name).HasMaxLength(200).IsRequired();
        builder.Property(variant => variant.Sku).HasMaxLength(64).IsRequired();
        builder.Property(variant => variant.Barcode).HasMaxLength(64);

        // The whole of "a SKU is never used twice" is this line; the check in the endpoint only
        // turns the common case into a friendlier answer.
        builder.HasIndex(variant => variant.Sku).IsUnique();
    }
}
