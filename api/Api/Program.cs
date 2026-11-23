using Business;
using Data;
using NJsonSchema;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing connection string 'Default'");

builder.Services.AddBusiness(connectionString);
builder.Services.AddValidation();
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

app.MapGet("/api/health", async (HealthService health) =>
    {
        var response = new HealthResponse(await health.IsDatabaseReachableAsync());
        return response.Database
            ? Results.Ok(response)
            : Results.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable);
    })
    .Produces<HealthResponse>()
    .Produces<HealthResponse>(StatusCodes.Status503ServiceUnavailable);

app.Run();

public record HealthResponse(bool Database);

// Lets the integration tests start the API.
public partial class Program;
