using System.Net;
using System.Net.Http.Json;

namespace IntegrationTests;

public class HealthTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Database_is_reachable()
    {
        var response = await factory.CreateClient().GetAsync("/api/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True((await response.Content.ReadFromJsonAsync<HealthResponse>())!.Database);
    }

    [Fact]
    public async Task Database_is_unreachable()
    {
        var client = factory
            .WithWebHostBuilder(b => b
                .UseSetting("ConnectionStrings:Default",
                    "Host=127.0.0.1;Port=1;Database=none;Username=none;Password=none;Timeout=2")
                .UseSetting("Migrate", "false"))
            .CreateClient();

        var response = await client.GetAsync("/api/health");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.False((await response.Content.ReadFromJsonAsync<HealthResponse>())!.Database);
    }
}
