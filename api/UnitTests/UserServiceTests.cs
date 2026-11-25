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

        public Task<User?> GetByEmailAsync(string email) =>
            Task.FromResult(Users.FirstOrDefault(u => u.Email == email));

        public Task<List<User>> GetAllAsync() => Task.FromResult(Users.ToList());
    }

    private static FakeUserRepository RepositoryWith(bool active) => new()
    {
        Users = { new User { Email = "anna@example.com", PasswordHash = Passwords.Hash("secret123"), Active = active } },
    };

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

    [Fact]
    public async Task Login_works_with_right_password()
    {
        var user = await new UserService(RepositoryWith(active: true)).LoginAsync(" Anna@Example.com ", "secret123");

        Assert.NotNull(user);
    }

    [Fact]
    public async Task Login_fails_with_wrong_password()
    {
        var user = await new UserService(RepositoryWith(active: true)).LoginAsync("anna@example.com", "wrong");

        Assert.Null(user);
    }

    [Fact]
    public async Task Login_fails_for_unknown_email()
    {
        var user = await new UserService(RepositoryWith(active: true)).LoginAsync("bob@example.com", "secret123");

        Assert.Null(user);
    }

    [Fact]
    public async Task Login_fails_for_inactive_user()
    {
        var user = await new UserService(RepositoryWith(active: false)).LoginAsync("anna@example.com", "secret123");

        Assert.Null(user);
    }
}
