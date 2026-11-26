using Data;

namespace Business;

// The methods that change data return an error message, or null when everything went well.
public class CalendarService(ICompanyDayRepository companyDays)
{
    // All days off in a year: public holidays and company days off.
    public async Task<List<DayOff>> GetDaysOffAsync(int year)
    {
        var company = (await companyDays.GetForYearAsync(year)).Select(d => new DayOff(d.Date, d.Name, d.Id));
        return PublicHolidays.For(year).Concat(company).OrderBy(d => d.Date).ToList();
    }

    public async Task<string?> AddCompanyDayAsync(DateOnly date, string name)
    {
        if (PublicHolidays.For(date.Year).Any(d => d.Date == date))
            return "The day is already a public holiday";

        if (await companyDays.GetByDateAsync(date) is not null)
            return "The day is already a company day off";

        await companyDays.AddAsync(new CompanyDay { Id = Guid.NewGuid(), Date = date, Name = name });
        return null;
    }

    // Returns false if there is no company day with this id.
    public Task<bool> RemoveCompanyDayAsync(Guid id) => companyDays.DeleteAsync(id);
}
