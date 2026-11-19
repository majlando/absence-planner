using Business;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBusiness(
    builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing connection string 'Default'"));
builder.Services.AddValidation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(doc => doc.Title = "Absence Planner API");

var app = builder.Build();

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

public partial class Program;
