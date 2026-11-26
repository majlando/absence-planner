using Business;
using Data;

namespace UnitTests;

public class CalendarServiceTests
{
    private class FakeCompanyDayRepository : ICompanyDayRepository
    {
        public List<CompanyDay> Days { get; } = [];

        public Task<List<CompanyDay>> GetForYearAsync(int year) =>
            Task.FromResult(Days.Where(d => d.Date.Year == year).ToList());

        public Task<CompanyDay?> GetByDateAsync(DateOnly date) =>
            Task.FromResult(Days.FirstOrDefault(d => d.Date == date));

        public Task AddAsync(CompanyDay day)
        {
            Days.Add(day);
            return Task.CompletedTask;
        }

        public Task<bool> DeleteAsync(Guid id) => Task.FromResult(Days.RemoveAll(d => d.Id == id) > 0);
    }

    private readonly FakeCompanyDayRepository _repository = new();
    private CalendarService Service => new(_repository);

    [Fact]
    public async Task Days_off_are_public_holidays_and_company_days_in_date_order()
    {
        await Service.AddCompanyDayAsync(new DateOnly(2025, 12, 24), "Christmas Eve");

        var days = await Service.GetDaysOffAsync(2025);

        Assert.Equal(11, days.Count);
        Assert.Equal(new DateOnly(2025, 12, 24), days[^3].Date);
        Assert.NotNull(days[^3].CompanyDayId);
        Assert.Null(days[^1].CompanyDayId);
    }

    [Fact]
    public async Task A_company_day_on_a_public_holiday_gives_an_error()
    {
        var error = await Service.AddCompanyDayAsync(new DateOnly(2025, 12, 25), "Christmas Day");

        Assert.NotNull(error);
        Assert.Empty(_repository.Days);
    }

    [Fact]
    public async Task The_same_company_day_twice_gives_an_error()
    {
        await Service.AddCompanyDayAsync(new DateOnly(2025, 12, 31), "New Year's Eve");

        var error = await Service.AddCompanyDayAsync(new DateOnly(2025, 12, 31), "New Year's Eve");

        Assert.NotNull(error);
        Assert.Single(_repository.Days);
    }
}
