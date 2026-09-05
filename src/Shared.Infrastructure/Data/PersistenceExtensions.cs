using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Shared.Infrastructure.Data;

public static class PersistenceExtensions
{
    private const string ConnectionStringName = "OrderSyncDb";

    /// <summary>
    /// The connection and the transaction that every module shares. Registered once by the host.
    /// </summary>
    public static IServiceCollection AddSharedPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = ReadConnectionString(configuration);

        services.AddScoped(_ => new DbConnectionAccessor(connectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    /// <summary>
    /// Registers one module's DbContext: on the request's shared connection, inside its own
    /// database schema, with its own migration history so modules migrate independently.
    /// </summary>
    public static IServiceCollection AddModuleDbContext<TContext>(this IServiceCollection services, string schema)
        where TContext : DbContext
    {
        services.AddDbContext<TContext>((serviceProvider, options) =>
            options.UseSqlServer(
                serviceProvider.GetRequiredService<DbConnectionAccessor>().Connection,
                sqlServer =>
                {
                    sqlServer.MigrationsAssembly(typeof(TContext).Assembly.FullName);
                    sqlServer.MigrationsHistoryTable(HistoryRepository.DefaultTableName, schema);
                }));

        // Also registered as plain DbContext so the unit of work can enlist it without knowing
        // which modules exist.
        services.AddScoped<DbContext>(serviceProvider => serviceProvider.GetRequiredService<TContext>());

        return services;
    }

    public static IServiceCollection AddDatabaseMigrator(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = ReadConnectionString(configuration);

        services.AddHostedService(serviceProvider => new DatabaseMigrator(
            serviceProvider.GetRequiredService<IServiceScopeFactory>(),
            connectionString,
            serviceProvider.GetRequiredService<ILogger<DatabaseMigrator>>()));

        return services;
    }

    private static string ReadConnectionString(IConfiguration configuration) =>
        configuration.GetConnectionString(ConnectionStringName)
        ?? throw new InvalidOperationException(
            $"Connection string '{ConnectionStringName}' is missing. Set it in appsettings.Development.json "
            + $"or through the ConnectionStrings__{ConnectionStringName} environment variable.");
}
