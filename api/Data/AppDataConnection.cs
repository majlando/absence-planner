using LinqToDB;
using LinqToDB.Data;

namespace Data;

public class AppDataConnection(DataOptions options) : DataConnection(options);
