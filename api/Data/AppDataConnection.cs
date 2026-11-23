using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.PostgreSQL;

namespace Data;

// The PostgreSQL version is fixed, because auto-detecting it would connect to the database right away.
public class AppDataConnection(string connectionString)
    : DataConnection(new DataOptions().UsePostgreSQL(connectionString, PostgreSQLVersion.v15));
