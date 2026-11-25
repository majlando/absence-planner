using System.Net;
using System.Net.Http.Json;
using Business;

namespace IntegrationTests;

public class UserManagementTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Theory]
    [InlineData("not-an-email", "secret123", Roles.Employee)]
    [InlineData("anna@example.com", "short", Roles.Employee)]
    [InlineData("anna@example.com", "secret123", "Boss")]
    public async Task Invalid_user_gives_400(string email, string password, string role)
    {
        var admin = await factory.LoginAsAdminAsync();

        var response = await admin.PostAsJsonAsync("/api/users", new CreateUserRequest(email, password, role));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Same_email_twice_gives_400()
    {
        var admin = await factory.LoginAsAdminAsync();
        var request = new CreateUserRequest("twice@example.com", "secret123", Roles.Employee);

        await admin.PostAsJsonAsync("/api/users", request);
        var response = await admin.PostAsJsonAsync("/api/users", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task New_user_can_log_in()
    {
        var admin = await factory.LoginAsAdminAsync();

        var response = await admin.PostAsJsonAsync("/api/users",
            new CreateUserRequest("new@example.com", "secret123", Roles.Employee));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        await factory.LoginAsync("new@example.com", "secret123");
    }

    [Fact]
    public async Task Deactivated_user_cannot_log_in()
    {
        var admin = await factory.LoginAsAdminAsync();
        await admin.PostAsJsonAsync("/api/users", new CreateUserRequest("leaving@example.com", "secret123", Roles.Employee));
        var users = await admin.GetFromJsonAsync<List<UserResponse>>("/api/users");
        var id = users!.Single(u => u.Email == "leaving@example.com").Id;

        await admin.PutAsJsonAsync($"/api/users/{id}", new UpdateUserRequest("leaving@example.com", Roles.Employee, false));
        var response = await factory.NewClient().PostAsJsonAsync("/api/auth/login",
            new LoginRequest("leaving@example.com", "secret123"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Employee_cannot_create_users()
    {
        var admin = await factory.LoginAsAdminAsync();
        await admin.PostAsJsonAsync("/api/users", new CreateUserRequest("emp@example.com", "secret123", Roles.Employee));
        var employee = await factory.LoginAsync("emp@example.com", "secret123");

        var response = await employee.PostAsJsonAsync("/api/users",
            new CreateUserRequest("other@example.com", "secret123", Roles.Admin));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task User_can_change_own_password()
    {
        var admin = await factory.LoginAsAdminAsync();
        await admin.PostAsJsonAsync("/api/users", new CreateUserRequest("pw@example.com", "secret123", Roles.Employee));
        var user = await factory.LoginAsync("pw@example.com", "secret123");

        var wrong = await user.PostAsJsonAsync("/api/auth/password", new ChangePasswordRequest("wrong-old", "new-secret"));
        var right = await user.PostAsJsonAsync("/api/auth/password", new ChangePasswordRequest("secret123", "new-secret"));

        Assert.Equal(HttpStatusCode.BadRequest, wrong.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, right.StatusCode);
        await factory.LoginAsync("pw@example.com", "new-secret");
    }
}
