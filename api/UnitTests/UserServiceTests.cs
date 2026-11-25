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

        // The user in the list is the same object, so it is already changed.
        public Task UpdateAsync(User user) => Task.CompletedTask;

        public Task<User?> GetByIdAsync(Guid id) => Task.FromResult(Users.FirstOrDefault(u => u.Id == id));

        public Task<User?> GetByEmailAsync(string email) =>
            Task.FromResult(Users.FirstOrDefault(u => u.Email == email));

        public Task<List<User>> GetAllAsync() => Task.FromResult(Users.ToList());
    }

    private static readonly Guid AnnaId = Guid.NewGuid();
    private static readonly Guid AdminId = Guid.NewGuid();

    private static FakeUserRepository RepositoryWith(bool active)
    {
        var repository = new FakeUserRepository();
        repository.Users.Add(new User
        {
            Id = AnnaId, Email = "anna@example.com", PasswordHash = Passwords.Hash("secret123"), Role = Roles.Employee, Active = active,
        });
        return repository;
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

    [Fact]
    public async Task Create_adds_an_active_user()
    {
        var repository = new FakeUserRepository();

        var error = await new UserService(repository).CreateAsync(" Bob@Example.com ", "secret123", Roles.Accounting);

        Assert.Null(error);
        var bob = Assert.Single(repository.Users);
        Assert.Equal("bob@example.com", bob.Email);
        Assert.Equal(Roles.Accounting, bob.Role);
        Assert.True(bob.Active);
        Assert.True(Passwords.Verify(bob.PasswordHash, "secret123"));
    }

    [Fact]
    public async Task Create_fails_when_email_is_in_use()
    {
        var repository = RepositoryWith(active: true);

        var error = await new UserService(repository).CreateAsync("ANNA@example.com", "secret123", Roles.Employee);

        Assert.Equal("The email is already in use", error);
        Assert.Single(repository.Users);
    }

    [Fact]
    public async Task Update_changes_email_role_and_active()
    {
        var repository = RepositoryWith(active: true);

        var error = await new UserService(repository).UpdateAsync(AdminId, AnnaId, "anna.b@example.com", Roles.Accounting, false);

        Assert.Null(error);
        var anna = repository.Users[0];
        Assert.Equal("anna.b@example.com", anna.Email);
        Assert.Equal(Roles.Accounting, anna.Role);
        Assert.False(anna.Active);
    }

    [Fact]
    public async Task Update_fails_when_email_belongs_to_another_user()
    {
        var repository = RepositoryWith(active: true);
        repository.Users.Add(new User { Id = Guid.NewGuid(), Email = "bob@example.com" });

        var error = await new UserService(repository).UpdateAsync(AdminId, AnnaId, "bob@example.com", Roles.Employee, true);

        Assert.Equal("The email is already in use", error);
    }

    [Fact]
    public async Task Admin_cannot_deactivate_themselves()
    {
        var repository = new FakeUserRepository();
        repository.Users.Add(new User { Id = AdminId, Email = "admin@example.com", Role = Roles.Admin, Active = true });

        var error = await new UserService(repository).UpdateAsync(AdminId, AdminId, "admin@example.com", Roles.Admin, false);

        Assert.Equal("You can't remove your own admin role or deactivate yourself", error);
        Assert.True(repository.Users[0].Active);
    }

    [Fact]
    public async Task Change_password_works_with_right_old_password()
    {
        var repository = RepositoryWith(active: true);

        var error = await new UserService(repository).ChangePasswordAsync(AnnaId, "secret123", "new-secret");

        Assert.Null(error);
        Assert.True(Passwords.Verify(repository.Users[0].PasswordHash, "new-secret"));
    }

    [Fact]
    public async Task Change_password_fails_with_wrong_old_password()
    {
        var repository = RepositoryWith(active: true);

        var error = await new UserService(repository).ChangePasswordAsync(AnnaId, "wrong", "new-secret");

        Assert.Equal("The old password is wrong", error);
        Assert.True(Passwords.Verify(repository.Users[0].PasswordHash, "secret123"));
    }
}
