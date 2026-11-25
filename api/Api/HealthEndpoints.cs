using Business;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this WebApplication app)
    {
        // Tells if the API can reach the database. Open to everyone.
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
    }
}

public record HealthResponse(bool Database);
