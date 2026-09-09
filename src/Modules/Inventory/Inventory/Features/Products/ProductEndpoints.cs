using Inventory.Data;
using Inventory.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Endpoints;

namespace Inventory.Features.Products;

/// <summary>
/// Catalog CRUD. Talks to the DbContext directly: there is no command to dispatch yet and no
/// rule to protect beyond the ones the aggregate already holds (work item 12 brings MediatR and
/// validation).
/// </summary>
internal sealed class ProductEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/inventory").WithTags("Inventory");

        group.MapPost("/products", CreateProduct);
        group.MapGet("/products", ListProducts);
        group.MapPost("/products/{productId:guid}/variants", AddVariant);
        group.MapPost("/variants/{variantId:guid}/deactivate", DeactivateVariant);
    }

    private static async Task<IResult> CreateProduct(
        CreateProductRequest request,
        InventoryDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (await SkuExistsAsync(dbContext, request.Sku, cancellationToken))
        {
            return SkuTaken(request.Sku);
        }

        Product product = Product.Create(
            request.Name,
            request.Description,
            request.VariantName,
            request.Sku,
            request.Barcode);

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Created($"/api/inventory/products/{product.Id}", ProductResponse.From(product));
    }

    private static async Task<IResult> ListProducts(InventoryDbContext dbContext, CancellationToken cancellationToken)
    {
        List<Product> products = await dbContext.Products
            .AsNoTracking()
            .Include(product => product.Variants)
            .OrderBy(product => product.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return Results.Ok(products.Select(ProductResponse.From));
    }

    private static async Task<IResult> AddVariant(
        Guid productId,
        AddVariantRequest request,
        InventoryDbContext dbContext,
        CancellationToken cancellationToken)
    {
        Product? product = await dbContext.Products
            .Include(product => product.Variants)
            .FirstOrDefaultAsync(product => product.Id == productId, cancellationToken);

        if (product is null)
        {
            return Results.NotFound();
        }

        if (await SkuExistsAsync(dbContext, request.Sku, cancellationToken))
        {
            return SkuTaken(request.Sku);
        }

        Variant variant = product.AddVariant(request.Name, request.Sku, request.Barcode);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Created($"/api/inventory/products/{product.Id}", VariantResponse.From(variant));
    }

    private static async Task<IResult> DeactivateVariant(
        Guid variantId,
        InventoryDbContext dbContext,
        CancellationToken cancellationToken)
    {
        Variant? variant = await dbContext.Variants
            .FirstOrDefaultAsync(variant => variant.Id == variantId, cancellationToken);

        if (variant is null)
        {
            return Results.NotFound();
        }

        variant.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    private static Task<bool> SkuExistsAsync(
        InventoryDbContext dbContext,
        string sku,
        CancellationToken cancellationToken) =>
        dbContext.Variants.AnyAsync(variant => variant.Sku == sku, cancellationToken);

    // Two callers racing both read "free" here and one of them hits the unique index instead;
    // that lands as a 500 until work item 12 maps the failure to this same answer.
    private static IResult SkuTaken(string sku) =>
        Results.Problem(
            title: "SKU is already in use",
            detail: $"SKU '{sku}' belongs to another variant. SKUs are never reused.",
            statusCode: StatusCodes.Status409Conflict);
}

internal sealed record CreateProductRequest(
    string Name,
    string? Description,
    string VariantName,
    string Sku,
    string? Barcode);

internal sealed record AddVariantRequest(string Name, string Sku, string? Barcode);

internal sealed record ProductResponse(Guid Id, string Name, string? Description, IReadOnlyList<VariantResponse> Variants)
{
    public static ProductResponse From(Product product) =>
        new(product.Id, product.Name, product.Description, [.. product.Variants.Select(VariantResponse.From)]);
}

internal sealed record VariantResponse(Guid Id, string Name, string Sku, string? Barcode, bool IsActive)
{
    public static VariantResponse From(Variant variant) =>
        new(variant.Id, variant.Name, variant.Sku, variant.Barcode, variant.IsActive);
}
