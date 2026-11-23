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
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = Passwords.Hash(password),
            Role = Roles.Admin,
            Active = true,
        });
    }
}
