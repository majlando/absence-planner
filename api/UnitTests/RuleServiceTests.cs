using Business;
using Data;

namespace UnitTests;

public class RuleServiceTests
{
    private class FakeRuleRepository(List<Rule> rules) : IRuleRepository
    {
        public Task<List<Rule>> GetAllAsync() => Task.FromResult(rules);
    }

    // The raised supplement went from 0.95% to 1.48% on 1 May 2024.
    private static readonly RuleService Service = new(new FakeRuleRepository(
    [
        new Rule { Id = Guid.NewGuid(), Name = RuleNames.RaisedSupplementPercent, Value = 0.95m, StartDate = new DateOnly(2020, 9, 1) },
        new Rule { Id = Guid.NewGuid(), Name = RuleNames.RaisedSupplementPercent, Value = 1.48m, StartDate = new DateOnly(2024, 5, 1) },
    ]));

    [Fact]
    public async Task Uses_the_old_value_before_the_change()
    {
        Assert.Equal(0.95m, await Service.ValueOnAsync(RuleNames.RaisedSupplementPercent, new DateOnly(2024, 4, 30)));
    }

    [Fact]
    public async Task Uses_the_new_value_from_the_start_date()
    {
        Assert.Equal(1.48m, await Service.ValueOnAsync(RuleNames.RaisedSupplementPercent, new DateOnly(2024, 5, 1)));
    }

    [Fact]
    public async Task No_value_before_the_first_start_date_throws()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Service.ValueOnAsync(RuleNames.RaisedSupplementPercent, new DateOnly(2020, 8, 31)));
    }
}
