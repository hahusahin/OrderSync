namespace Shared.Infrastructure.Data;

/// <summary>
/// Spans one transaction across several module DbContexts. A single DbContext already is a unit
/// of work; this exists only because order creation writes through two of them and nothing in
/// EF Core commits both together.
/// </summary>
public interface IUnitOfWork
{
    Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}

public interface IUnitOfWorkTransaction : IAsyncDisposable
{
    /// <summary>
    /// Saves every enlisted context and commits. Disposing without calling this rolls back.
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);
}
