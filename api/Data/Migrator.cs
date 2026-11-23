using LinqToDB.Data;

namespace Data;

// Runs each Migrations/*.sql file once, in name order.
public static class Migrator
{
    public static async Task RunAsync(string connectionString)
    {
        await using var db = new AppDataConnection(connectionString);

        await db.ExecuteAsync("CREATE TABLE IF NOT EXISTS migrations (name text PRIMARY KEY)");
        var done = db.Query<string>("SELECT name FROM migrations").ToList();

        var assembly = typeof(Migrator).Assembly;
        var files = assembly.GetManifestResourceNames().Where(name => name.EndsWith(".sql")).Order();

        foreach (var file in files.Where(file => !done.Contains(file)))
        {
            using var reader = new StreamReader(assembly.GetManifestResourceStream(file)!);
            await using var transaction = await db.BeginTransactionAsync();
            await db.ExecuteAsync(await reader.ReadToEndAsync());
            await db.ExecuteAsync("INSERT INTO migrations (name) VALUES (@name)", new DataParameter("name", file));
            await transaction.CommitAsync();
        }
    }
}
