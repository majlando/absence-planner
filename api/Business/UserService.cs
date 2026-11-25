using Data;

namespace Business;

// The methods that change users return an error message, or null when everything went well.
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

    public Task<User?> GetAsync(Guid id) => users.GetByIdAsync(id);

    public async Task<string?> CreateAsync(string email, string password, string role)
    {
        if (await users.GetByEmailAsync(Normalize(email)) is not null)
            return "The email is already in use";

        await users.AddAsync(new User
        {
            Id = Guid.NewGuid(),
            Email = Normalize(email),
            PasswordHash = Passwords.Hash(password),
            Role = role,
            Active = true,
        });
        return null;
    }

    // currentUserId is the admin who makes the change.
    public async Task<string?> UpdateAsync(Guid currentUserId, Guid id, string email, string role, bool active)
    {
        var user = await users.GetByIdAsync(id);
        if (user is null)
            return "The user doesn't exist";

        var other = await users.GetByEmailAsync(Normalize(email));
        if (other is not null && other.Id != id)
            return "The email is already in use";

        // Otherwise the admin could lock themselves out.
        if (id == currentUserId && (role != Roles.Admin || !active))
            return "You can't remove your own admin role or deactivate yourself";

        user.Email = Normalize(email);
        user.Role = role;
        user.Active = active;
        await users.UpdateAsync(user);
        return null;
    }

    public async Task<string?> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
    {
        var user = await users.GetByIdAsync(userId);
        if (user is null || !Passwords.Verify(user.PasswordHash, oldPassword))
            return "The old password is wrong";

        user.PasswordHash = Passwords.Hash(newPassword);
        await users.UpdateAsync(user);
        return null;
    }

    private static string Normalize(string email) => email.Trim().ToLowerInvariant();
}
