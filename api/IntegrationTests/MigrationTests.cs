using Data;

namespace IntegrationTests;

public class MigrationTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Migrations_can_run_twice()
    {
        await Migrator.RunAsync(factory.ConnectionString);
        await Migrator.RunAsync(factory.ConnectionString);
    }
}
