using LinqToDB.Mapping;

namespace Data;

// A day off for everyone in the company, e.g. 24 December.
[Table("company_days")]
public class CompanyDay
{
    [Column("id"), PrimaryKey] public Guid Id { get; set; }
    [Column("date")] public DateOnly Date { get; set; }
    [Column("name")] public string Name { get; set; } = "";
}
