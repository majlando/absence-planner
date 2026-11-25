using Data;

namespace Business;

public class UserService(IUserRepository users)
{
    // Creates an admin the first time the system starts, so someone can log in.
    public async Task CreateFirstAdminAsync(string email, string password)
    {
        if (await users.AnyAsync())
            return;

        await users.AddAsync(new User
        {
            Id = Guid.NewGuid(),
            Email = Normalize(email),
            PasswordHash = Passwords.Hash(password),
            Role = Roles.Admin,
            Active = true,
        });
    }

    // Returns the user if the email and password are right and the user is active, otherwise null.
    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await users.GetByEmailAsync(Normalize(email));
        if (user is null || !user.Active || !Passwords.Verify(user.PasswordHash, password))
            return null;

        return user;
    }

    public Task<List<User>> GetAllAsync() => users.GetAllAsync();

    private static string Normalize(string email) => email.Trim().ToLowerInvariant();
}
