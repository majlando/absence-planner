using Data;

namespace Business;

public class RuleService(IRuleRepository rules)
{
    public Task<List<Rule>> GetAllAsync() => rules.GetAllAsync();

    // The value of a rule on a date: the row with the latest start date on or before that date.
    public async Task<decimal> ValueOnAsync(string name, DateOnly date)
    {
        var rule = (await rules.GetAllAsync())
            .Where(r => r.Name == name && r.StartDate <= date)
            .MaxBy(r => r.StartDate);

        return rule?.Value ?? throw new InvalidOperationException($"Rule {name} has no value on {date}");
    }
}
