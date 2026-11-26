using LinqToDB;
using LinqToDB.Async;

namespace Data;

public interface ICompanyDayRepository
{
    Task<List<CompanyDay>> GetForYearAsync(int year);
    Task<CompanyDay?> GetByDateAsync(DateOnly date);
    Task AddAsync(CompanyDay day);
    Task<bool> DeleteAsync(Guid id);
}

public class CompanyDayRepository(AppDataConnection db) : ICompanyDayRepository
{
    public Task<List<CompanyDay>> GetForYearAsync(int year) =>
        db.GetTable<CompanyDay>()
            .Where(d => d.Date >= new DateOnly(year, 1, 1) && d.Date <= new DateOnly(year, 12, 31))
            .ToListAsync();

    public Task<CompanyDay?> GetByDateAsync(DateOnly date) =>
        db.GetTable<CompanyDay>().FirstOrDefaultAsync(d => d.Date == date);

    public Task AddAsync(CompanyDay day) => db.InsertAsync(day);

    public async Task<bool> DeleteAsync(Guid id) =>
        await db.GetTable<CompanyDay>().DeleteAsync(d => d.Id == id) > 0;
}
