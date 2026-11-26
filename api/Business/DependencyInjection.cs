using Data;
using Microsoft.Extensions.DependencyInjection;

namespace Business;

public static class DependencyInjection
{
    public static IServiceCollection AddBusiness(this IServiceCollection services, string connectionString) => services
        .AddData(connectionString)
        .AddScoped<HealthService>()
        .AddScoped<UserService>()
        .AddScoped<RuleService>();
}
