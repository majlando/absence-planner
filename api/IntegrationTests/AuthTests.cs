using System.Net;
using System.Net.Http.Json;
using Business;
using Data;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTests;

public class AuthTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    // A client that keeps cookies, like a browser.
    private HttpClient NewClient() => factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });

    private async Task<HttpClient> LoginAsync(string email, string password)
    {
        var client = NewClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, password));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return client;
    }

    private async Task AddUserAsync(string email, string password, string role)
    {
        NewClient(); // starts the API, so the migrations have run
        await using var db = new AppDataConnection(factory.ConnectionString);
        await new UserRepository(db).AddAsync(new User
        {
            Id = Guid.NewGuid(), Email = email, PasswordHash = Passwords.Hash(password), Role = role, Active = true,
        });
    }

    [Fact]
    public async Task Not_logged_in_gives_401()
    {
        var response = await NewClient().GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Wrong_password_gives_401()
    {
        var response = await NewClient().PostAsJsonAsync("/api/auth/login",
            new LoginRequest(ApiFactory.AdminEmail, "wrong-password"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_then_me_returns_the_user()
    {
        var client = await LoginAsync(ApiFactory.AdminEmail, ApiFactory.AdminPassword);

        var me = await client.GetFromJsonAsync<MeResponse>("/api/auth/me");

        Assert.Equal(ApiFactory.AdminEmail, me!.Email);
        Assert.Equal(Roles.Admin, me.Role);
    }

    [Fact]
    public async Task Logout_removes_the_login()
    {
        var client = await LoginAsync(ApiFactory.AdminEmail, ApiFactory.AdminPassword);

        await client.PostAsync("/api/auth/logout", null);
        var response = await client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Admin_can_list_users()
    {
        var client = await LoginAsync(ApiFactory.AdminEmail, ApiFactory.AdminPassword);

        var response = await client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Wrong_role_gives_403()
    {
        await AddUserAsync("employee@example.com", "employee-password", Roles.Employee);
        var client = await LoginAsync("employee@example.com", "employee-password");

        var response = await client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
