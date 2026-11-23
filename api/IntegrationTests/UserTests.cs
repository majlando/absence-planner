using Business;
using Data;
using LinqToDB;
using LinqToDB.Async;

namespace IntegrationTests;

public class UserTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task First_admin_is_created_on_startup()
    {
        factory.CreateClient(); // starts the API

        await using var db = new AppDataConnection(factory.ConnectionString);
        var admin = await db.GetTable<User>().SingleAsync();

        Assert.Equal(ApiFactory.AdminEmail, admin.Email);
        Assert.Equal(Roles.Admin, admin.Role);
        Assert.True(Passwords.Verify(admin.PasswordHash, ApiFactory.AdminPassword));
    }
}
