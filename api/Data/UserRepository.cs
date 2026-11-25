using LinqToDB;
using LinqToDB.Async;

namespace Data;

public interface IUserRepository
{
    Task<bool> AnyAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetAllAsync();
}

public class UserRepository(AppDataConnection db) : IUserRepository
{
    public Task<bool> AnyAsync() => db.GetTable<User>().AnyAsync();

    public Task AddAsync(User user) => db.InsertAsync(user);

    public Task UpdateAsync(User user) => db.UpdateAsync(user);

    public Task<User?> GetByIdAsync(Guid id) =>
        db.GetTable<User>().FirstOrDefaultAsync(u => u.Id == id);

    public Task<User?> GetByEmailAsync(string email) =>
        db.GetTable<User>().FirstOrDefaultAsync(u => u.Email == email);

    public Task<List<User>> GetAllAsync() => db.GetTable<User>().OrderBy(u => u.Email).ToListAsync();
}
