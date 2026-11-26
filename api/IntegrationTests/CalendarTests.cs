using System.Net;
using System.Net.Http.Json;
using Business;

namespace IntegrationTests;

public class CalendarTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Employee_can_get_the_days_off_for_a_year()
    {
        var employee = await CreateEmployeeAsync("calendar@example.com");

        var days = await employee.GetFromJsonAsync<List<DayOffResponse>>("/api/calendar/2025");

        Assert.Contains(days!, d => d.Date == new DateOnly(2025, 12, 25) && d.CompanyDayId == null);
    }

    [Fact]
    public async Task Admin_can_add_and_remove_a_company_day()
    {
        var admin = await factory.LoginAsAdminAsync();

        var added = await admin.PostAsJsonAsync("/api/calendar/company-days",
            new CompanyDayRequest(new DateOnly(2026, 12, 24), "Christmas Eve"));
        var days = await admin.GetFromJsonAsync<List<DayOffResponse>>("/api/calendar/2026");
        var id = days!.Single(d => d.Date == new DateOnly(2026, 12, 24)).CompanyDayId;
        var removed = await admin.DeleteAsync($"/api/calendar/company-days/{id}");
        var after = await admin.GetFromJsonAsync<List<DayOffResponse>>("/api/calendar/2026");

        Assert.Equal(HttpStatusCode.NoContent, added.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, removed.StatusCode);
        Assert.DoesNotContain(after!, d => d.Date == new DateOnly(2026, 12, 24));
    }

    [Fact]
    public async Task Company_day_on_a_public_holiday_gives_400()
    {
        var admin = await factory.LoginAsAdminAsync();

        var response = await admin.PostAsJsonAsync("/api/calendar/company-days",
            new CompanyDayRequest(new DateOnly(2026, 12, 25), "Christmas Day"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Employee_cannot_add_a_company_day()
    {
        var employee = await CreateEmployeeAsync("no-days@example.com");

        var response = await employee.PostAsJsonAsync("/api/calendar/company-days",
            new CompanyDayRequest(new DateOnly(2026, 12, 31), "New Year's Eve"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private async Task<HttpClient> CreateEmployeeAsync(string email)
    {
        var admin = await factory.LoginAsAdminAsync();
        await admin.PostAsJsonAsync("/api/users", new CreateUserRequest(email, "secret123", Roles.Employee));
        return await factory.LoginAsync(email, "secret123");
    }
}
