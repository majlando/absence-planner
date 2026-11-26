using System.Net;
using System.Net.Http.Json;
using Business;

namespace IntegrationTests;

public class AuthTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Not_logged_in_gives_401()
    {
        var response = await factory.NewClient().GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Wrong_password_gives_401()
    {
        var response = await factory.NewClient().PostAsJsonAsync("/api/auth/login",
            new LoginRequest(ApiFactory.AdminEmail, "wrong-password"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_without_email_and_password_gives_400()
    {
        var response = await factory.NewClient().PostAsJsonAsync("/api/auth/login", new { });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_then_me_returns_the_user()
    {
        var client = await factory.LoginAsAdminAsync();

        var me = await client.GetFromJsonAsync<MeResponse>("/api/auth/me");

        Assert.Equal(ApiFactory.AdminEmail, me!.Email);
        Assert.Equal(Roles.Admin, me.Role);
    }

    [Fact]
    public async Task Logout_removes_the_login()
    {
        var client = await factory.LoginAsAdminAsync();

        await client.PostAsync("/api/auth/logout", null);
        var response = await client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Admin_can_list_users()
    {
        var client = await factory.LoginAsAdminAsync();

        var response = await client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Wrong_role_gives_403()
    {
        var admin = await factory.LoginAsAdminAsync();
        await admin.PostAsJsonAsync("/api/users",
            new CreateUserRequest("employee@example.com", "employee-password", Roles.Employee));
        var client = await factory.LoginAsync("employee@example.com", "employee-password");

        var response = await client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
