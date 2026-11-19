using LinqToDB;
using LinqToDB.DataProvider.PostgreSQL;
using Microsoft.Extensions.DependencyInjection;

namespace Data;

public static class DependencyInjection
{
    public static IServiceCollection AddData(this IServiceCollection services, string connectionString)
    {
        // Pinned dialect (postgres:17 → v15 is the newest dialect it supports). AutoDetect would open a
        // connection while constructing AppDataConnection, so a down database would throw during DI resolution.
        var options = new DataOptions().UsePostgreSQL(connectionString, PostgreSQLVersion.v15);

        return services
            .AddScoped(_ => new AppDataConnection(options))
            .AddScoped<IHealthRepository, HealthRepository>();
    }
}
