using Business;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        // Only admins may see the list of users.
        app.MapGet("/api/users", async (UserService users) =>
                (await users.GetAllAsync()).Select(u => new UserResponse(u.Id, u.Email, u.Role, u.Active)))
            .RequireAuthorization(policy => policy.RequireRole(Roles.Admin))
            .Produces<IEnumerable<UserResponse>>();
    }
}

public record UserResponse(Guid Id, string Email, string Role, bool Active);
