using Shared.Kernel;

namespace Inventory.Domain;

/// <summary>
/// The thing that is actually stocked and sold. Work item 07's stock row hangs off this Id.
/// </summary>
public sealed class Variant : Entity
{
    public Guid ProductId { get; private init; }

    public string Name { get; private set; } = null!;

    /// <summary>Our own business code, uniquely indexed - never the key, and never reused.</summary>
    public string Sku { get; private set; } = null!;

    /// <summary>The manufacturer's code. Optional: own-brand goods often have none.</summary>
    public string? Barcode { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private init; }

    private Variant(Guid id, Guid productId, string name, string sku, string? barcode) : base(id)
    {
        ProductId = productId;
        Name = name;
        Sku = sku;
        Barcode = barcode;
        IsActive = true;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    private Variant()
    {
    }

    internal static Variant Create(Guid productId, string name, string sku, string? barcode) =>
        new(Guid.CreateVersion7(), productId, name, sku, barcode);

    /// <summary>
    /// A closed variant is deactivated, not deleted: the row keeps standing, so the unique index
    /// keeps rejecting its SKU, and the ledger never adds up two different goods under one code.
    /// </summary>
    public void Deactivate() => IsActive = false;
}
