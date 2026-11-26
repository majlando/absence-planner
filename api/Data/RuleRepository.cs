using LinqToDB;
using LinqToDB.Async;

namespace Data;

public interface IRuleRepository
{
    Task<List<Rule>> GetAllAsync();
}

public class RuleRepository(AppDataConnection db) : IRuleRepository
{
    public Task<List<Rule>> GetAllAsync() =>
        db.GetTable<Rule>().OrderBy(r => r.Name).ThenBy(r => r.StartDate).ToListAsync();
}
