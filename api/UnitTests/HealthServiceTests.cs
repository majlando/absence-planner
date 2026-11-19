using Business;
using Data;

namespace UnitTests;

public class HealthServiceTests
{
    private class FakeHealthRepository(bool canConnect) : IHealthRepository
    {
        public Task<bool> CanConnectAsync() => Task.FromResult(canConnect);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Reports_database_reachability(bool canConnect)
    {
        var service = new HealthService(new FakeHealthRepository(canConnect));

        Assert.Equal(canConnect, await service.IsDatabaseReachableAsync());
    }
}
