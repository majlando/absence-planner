using System.Net;
using System.Net.Http.Json;
using Business;

namespace IntegrationTests;

public class RuleTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Admin_can_list_the_rules_from_the_migration()
    {
        var admin = await factory.LoginAsAdminAsync();

        var rules = await admin.GetFromJsonAsync<List<RuleResponse>>("/api/rules");

        var raised = rules!.Where(r => r.Name == RuleNames.RaisedSupplementPercent).ToList();
        Assert.Equal([0.95m, 1.48m], raised.Select(r => r.Value));
        Assert.Equal(new DateOnly(2024, 5, 1), raised[1].StartDate);
    }

    [Fact]
    public async Task Employee_cannot_list_the_rules()
    {
        var admin = await factory.LoginAsAdminAsync();
        await admin.PostAsJsonAsync("/api/users", new CreateUserRequest("rules@example.com", "secret123", Roles.Employee));
        var employee = await factory.LoginAsync("rules@example.com", "secret123");

        var response = await employee.GetAsync("/api/rules");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
