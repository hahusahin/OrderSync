using Shared.Kernel;

namespace Inventory.Domain;

/// <summary>
/// A grouping of variants. Deliberately carries no quantity: "40 of this product" cannot be
/// acted on, because the order arrives for size 42.
/// </summary>
public sealed class Product : AggregateRoot
{
    private readonly List<Variant> _variants = [];

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private init; }

    /// <summary>Read-only to callers: a variant is only ever added through <see cref="AddVariant"/>.</summary>
    public IReadOnlyList<Variant> Variants => _variants;

    private Product(Guid id, string name, string? description) : base(id)
    {
        Name = name;
        Description = description;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    // EF Core materialises through this; every value is then read from the row.
    private Product()
    {
    }

    /// <summary>
    /// A product is never created without a variant, so no second code path for "products that
    /// have no variants" can appear later.
    /// </summary>
    public static Product Create(string name, string? description, string variantName, string sku, string? barcode)
    {
        // Version 7 is time-ordered, so rows are appended to the end of the clustered index
        // instead of being inserted into the middle of it.
        Product product = new(Guid.CreateVersion7(), name, description);
        product.AddVariant(variantName, sku, barcode);

        return product;
    }

    public Variant AddVariant(string name, string sku, string? barcode)
    {
        Variant variant = Variant.Create(Id, name, sku, barcode);
        _variants.Add(variant);

        return variant;
    }
}
