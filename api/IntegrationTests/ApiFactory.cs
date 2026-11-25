using System.Net;
using System.Net.Http.Json;
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

    // A client that keeps cookies, like a browser.
    public HttpClient NewClient() => CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });

    public async Task<HttpClient> LoginAsync(string email, string password)
    {
        var client = NewClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, password));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return client;
    }

    public Task<HttpClient> LoginAsAdminAsync() => LoginAsync(AdminEmail, AdminPassword);

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
