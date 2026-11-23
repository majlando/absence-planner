using Microsoft.Extensions.DependencyInjection;

namespace Data;

public static class DependencyInjection
{
    public static IServiceCollection AddData(this IServiceCollection services, string connectionString) => services
        .AddScoped(_ => new AppDataConnection(connectionString))
        .AddScoped<IHealthRepository, HealthRepository>()
        .AddScoped<IUserRepository, UserRepository>();
}
