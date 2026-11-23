using Business;
using Data;

namespace UnitTests;

public class UserServiceTests
{
    private class FakeUserRepository : IUserRepository
    {
        public List<User> Users { get; } = [];

        public Task<bool> AnyAsync() => Task.FromResult(Users.Count > 0);

        public Task AddAsync(User user)
        {
            Users.Add(user);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Creates_admin_when_there_are_no_users()
    {
        var repository = new FakeUserRepository();

        await new UserService(repository).CreateFirstAdminAsync("Admin@Example.com", "secret123");

        var admin = Assert.Single(repository.Users);
        Assert.Equal("admin@example.com", admin.Email);
        Assert.Equal(Roles.Admin, admin.Role);
        Assert.True(admin.Active);
        Assert.True(Passwords.Verify(admin.PasswordHash, "secret123"));
    }

    [Fact]
    public async Task Does_nothing_when_users_exist()
    {
        var repository = new FakeUserRepository();
        repository.Users.Add(new User { Email = "someone@example.com" });

        await new UserService(repository).CreateFirstAdminAsync("admin@example.com", "secret123");

        Assert.Single(repository.Users);
    }
}
