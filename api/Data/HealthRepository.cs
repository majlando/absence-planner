using System.Data.Common;
using LinqToDB.Data;

namespace Data;

public interface IHealthRepository
{
    Task<bool> CanConnectAsync();
}

public class HealthRepository(AppDataConnection db) : IHealthRepository
{
    public async Task<bool> CanConnectAsync()
    {
        try
        {
            return await db.ExecuteAsync<int>("SELECT 1") == 1;
        }
        catch (DbException)
        {
            return false;
        }
    }
}
