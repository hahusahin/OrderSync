using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Shared.Infrastructure.Data;

/// <summary>
/// Creates the database if it is missing and applies every module's pending migrations at
/// startup. Development only - in any other environment migrations are applied deliberately,
/// not by the process that serves traffic.
/// </summary>
internal sealed class DatabaseMigrator(
    IServiceScopeFactory scopeFactory,
    string connectionString,
    ILogger<DatabaseMigrator> logger) : IHostedService
{
    private const int MaxAttempts = 10;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(3);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await WithRetryAsync("database", () => CreateDatabaseIfMissingAsync(cancellationToken), cancellationToken);

        // Async scope: the connection accessor is IAsyncDisposable, and a synchronous scope
        // refuses to dispose it.
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();

        foreach (DbContext context in scope.ServiceProvider.GetServices<DbContext>())
        {
            string name = context.GetType().Name;

            await WithRetryAsync(name, () => context.Database.MigrateAsync(cancellationToken), cancellationToken);
            logger.LogInformation("{Context} is up to date.", name);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <summary>
    /// EF Core would create the database itself, but it cannot here: the contexts are handed a
    /// connection object bound to a database that does not exist yet, and the failed open costs
    /// that object its credentials before EF can retry through master.
    /// </summary>
    private async Task CreateDatabaseIfMissingAsync(CancellationToken cancellationToken)
    {
        SqlConnectionStringBuilder builder = new(connectionString);
        string database = builder.InitialCatalog;
        builder.InitialCatalog = "master";

        await using SqlConnection master = new(builder.ConnectionString);
        await master.OpenAsync(cancellationToken);

        await using SqlCommand command = master.CreateCommand();

        // A database name cannot be a parameter, so both spellings of it are escaped by hand.
        command.CommandText =
            $"IF DB_ID(N'{database.Replace("'", "''")}') IS NULL CREATE DATABASE [{database.Replace("]", "]]")}];";

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task WithRetryAsync(string subject, Func<Task> operation, CancellationToken cancellationToken)
    {
        for (int attempt = 1; ; attempt++)
        {
            try
            {
                await operation();
                return;
            }
            catch (SqlException exception) when (attempt < MaxAttempts && IsServerNotReadyYet(exception))
            {
                logger.LogWarning(
                    "Waiting for SQL Server before touching {Subject} (attempt {Attempt}/{MaxAttempts}): {Message}",
                    subject, attempt, MaxAttempts, exception.Message);

                await Task.Delay(RetryDelay, cancellationToken);
            }
        }
    }

    // The container reports healthy a few seconds before it accepts connections on a cold start.
    // A rejected login or a bad database name is not that, and is thrown at once rather than
    // buried under half a minute of retries.
    private static bool IsServerNotReadyYet(SqlException exception) =>
        exception.IsTransient || exception.Number is 53 or -2 or 10061;
}
