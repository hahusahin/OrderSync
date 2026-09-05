using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Shared.Infrastructure.Data;

internal sealed class UnitOfWork(DbConnectionAccessor connectionAccessor, IEnumerable<DbContext> contexts)
    : IUnitOfWork
{
    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        DbConnection connection = await connectionAccessor.OpenAsync(cancellationToken);
        DbTransaction transaction = await connection.BeginTransactionAsync(cancellationToken);

        // Every module context is enlisted, not only the ones touched so far: a context created
        // later in the request would otherwise run outside this transaction and commit on its own.
        DbContext[] enlisted = contexts.ToArray();

        foreach (DbContext context in enlisted)
        {
            await context.Database.UseTransactionAsync(transaction, cancellationToken);
        }

        return new UnitOfWorkTransaction(transaction, enlisted);
    }

    private sealed class UnitOfWorkTransaction(DbTransaction transaction, IReadOnlyCollection<DbContext> contexts)
        : IUnitOfWorkTransaction
    {
        private bool _committed;

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            foreach (DbContext context in contexts)
            {
                await context.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
            _committed = true;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_committed)
            {
                await transaction.RollbackAsync();
            }

            await transaction.DisposeAsync();
        }
    }
}
