using LinqToDB;
using LinqToDB.Async;

namespace Data;

public interface IUserRepository
{
    Task<bool> AnyAsync();
    Task AddAsync(User user);
}

public class UserRepository(AppDataConnection db) : IUserRepository
{
    public Task<bool> AnyAsync() => db.GetTable<User>().AnyAsync();

    public Task AddAsync(User user) => db.InsertAsync(user);
}
