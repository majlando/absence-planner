using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Business;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var auth = app.MapGroup("/api/auth");

        // Checks email and password and sets the login cookie.
        auth.MapPost("/login", async (LoginRequest request, UserService users, HttpContext http) =>
            {
                var user = await users.LoginAsync(request.Email, request.Password);
                if (user is null)
                    return Results.Unauthorized();

                var identity = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                ], CookieAuthenticationDefaults.AuthenticationScheme);
                await http.SignInAsync(new ClaimsPrincipal(identity));

                return Results.Ok(new MeResponse(user.Id, user.Email, user.Role));
            })
            .AllowAnonymous()
            .Produces<MeResponse>()
            .Produces(StatusCodes.Status401Unauthorized);

        // Removes the login cookie.
        auth.MapPost("/logout", async (HttpContext http) =>
            {
                await http.SignOutAsync();
                return Results.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent);

        // Returns the logged-in user.
        auth.MapGet("/me", (ClaimsPrincipal user) =>
            {
                var email = user.FindFirstValue(ClaimTypes.Email)!;
                var role = user.FindFirstValue(ClaimTypes.Role)!;
                return new MeResponse(user.UserId(), email, role);
            })
            .Produces<MeResponse>();

        // Changes the logged-in user's own password.
        auth.MapPost("/password", async (ChangePasswordRequest request, UserService users, ClaimsPrincipal user) =>
                await users.ChangePasswordAsync(user.UserId(), request.OldPassword, request.NewPassword) is { } error
                    ? Errors.BadRequest(error)
                    : Results.NoContent())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem();
    }

    // The id of the logged-in user, saved in the cookie at login.
    public static Guid UserId(this ClaimsPrincipal user) =>
        Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Runs on every request with a cookie. Deactivated users and users with a new role must log in again.
    public static async Task CheckUserAsync(CookieValidatePrincipalContext context)
    {
        var me = context.Principal!;
        var users = context.HttpContext.RequestServices.GetRequiredService<UserService>();
        var user = await users.GetAsync(me.UserId());
        if (user is null || !user.Active || user.Role != me.FindFirstValue(ClaimTypes.Role))
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync();
        }
    }
}

public record LoginRequest([Required] string Email, [Required] string Password);

public record ChangePasswordRequest([Required] string OldPassword, [Required, MinLength(8)] string NewPassword);

public record MeResponse(Guid Id, string Email, string Role);
