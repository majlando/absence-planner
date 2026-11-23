using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace IntegrationTests;

public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _db = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public const string AdminEmail = "admin@example.com";
    public const string AdminPassword = "admin-password";

    public string ConnectionString => _db.GetConnectionString();

    public Task InitializeAsync() => _db.StartAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder) => builder
        .UseSetting("ConnectionStrings:Default", ConnectionString)
        .UseSetting("ADMIN_EMAIL", AdminEmail)
        .UseSetting("ADMIN_PASSWORD", AdminPassword);

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _db.DisposeAsync();
    }
}
