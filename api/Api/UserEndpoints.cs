using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Business;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        // Only admins may use these endpoints.
        var users = app.MapGroup("/api/users").RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        users.MapGet("", async (UserService service) =>
                (await service.GetAllAsync()).Select(u => new UserResponse(u.Id, u.Email, u.Role, u.Active)))
            .Produces<IEnumerable<UserResponse>>();

        users.MapGet("/{id:guid}", async (Guid id, UserService service) =>
                await service.GetAsync(id) is { } u
                    ? Results.Ok(new UserResponse(u.Id, u.Email, u.Role, u.Active))
                    : Results.NotFound())
            .Produces<UserResponse>()
            .Produces(StatusCodes.Status404NotFound);

        users.MapPost("", async (CreateUserRequest request, UserService service) =>
                await service.CreateAsync(request.Email, request.Password, request.Role) is { } error
                    ? Errors.BadRequest(error)
                    : Results.NoContent())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem();

        users.MapPut("/{id:guid}", async (Guid id, UpdateUserRequest request, UserService service, ClaimsPrincipal me) =>
                await service.UpdateAsync(me.UserId(), id, request.Email, request.Role, request.Active) is { } error
                    ? Errors.BadRequest(error)
                    : Results.NoContent())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem();
    }
}

public record UserResponse(Guid Id, string Email, string Role, bool Active);

public record CreateUserRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password,
    [Required, AllowedValues(Roles.Employee, Roles.Accounting, Roles.Admin)] string Role);

public record UpdateUserRequest(
    [Required, EmailAddress] string Email,
    [Required, AllowedValues(Roles.Employee, Roles.Accounting, Roles.Admin)] string Role,
    bool Active);
