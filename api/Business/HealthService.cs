using Data;

namespace Business;

public class HealthService(IHealthRepository repository)
{
    public Task<bool> IsDatabaseReachableAsync() => repository.CanConnectAsync();
}
