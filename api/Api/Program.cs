using Business;
using Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using NJsonSchema;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing connection string 'Default'");

builder.Services.AddBusiness(connectionString);
builder.Services.AddValidation();

// Login with a cookie. The API answers 401/403 instead of redirecting to a login page.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth";
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

// Every endpoint needs login, unless it says AllowAnonymous.
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(doc =>
{
    doc.Title = "Absence Planner API";
    // Non-nullable properties are required, so the TypeScript client doesn't make them optional.
    doc.PostProcess = d =>
    {
        foreach (var property in d.Components.Schemas.Values.SelectMany(s => s.Properties.Values))
            if (!property.IsNullable(SchemaType.OpenApi3))
                property.IsRequired = true;
    };
});

var app = builder.Build();

// Set up the database: run migrations and create the first admin.
// Tests can turn this off with the setting Migrate=false.
if (app.Configuration.GetValue("Migrate", true))
{
    await Migrator.RunAsync(connectionString);

    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<UserService>().CreateFirstAdminAsync(
        app.Configuration["ADMIN_EMAIL"] ?? throw new InvalidOperationException("Missing ADMIN_EMAIL"),
        app.Configuration["ADMIN_PASSWORD"] ?? throw new InvalidOperationException("Missing ADMIN_PASSWORD"));
}

app.UseOpenApi();
app.UseSwaggerUi();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/health", async (HealthService health) =>
    {
        var response = new HealthResponse(await health.IsDatabaseReachableAsync());
        return response.Database
            ? Results.Ok(response)
            : Results.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable);
    })
    .Produces<HealthResponse>()
    .Produces<HealthResponse>(StatusCodes.Status503ServiceUnavailable)
    .AllowAnonymous();

app.MapAuthEndpoints();
app.MapUserEndpoints();

app.Run();

public record HealthResponse(bool Database);

// Lets the integration tests start the API.
public partial class Program;
