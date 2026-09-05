using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Shared.Infrastructure.Data;

/// <summary>
/// The single database connection of one request. Every module's DbContext is built on this
/// object, so a transaction begun here covers all of them - one connection is one SQL Server
/// transaction, with no distributed transaction coordinator involved.
/// </summary>
/// <remarks>
/// A connection cannot serve two queries at once, so a request that fans work out to background
/// threads would corrupt this. Handlers stay on the request's own thread.
/// </remarks>
public sealed class DbConnectionAccessor(string connectionString) : IAsyncDisposable
{
    private readonly SqlConnection _connection = new(connectionString);

    public DbConnection Connection => _connection;

    public async ValueTask<DbConnection> OpenAsync(CancellationToken cancellationToken = default)
    {
        if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync(cancellationToken);
        }

        return _connection;
    }

    public ValueTask DisposeAsync() => _connection.DisposeAsync();
}
