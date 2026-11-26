using LinqToDB.Mapping;

namespace Data;

[Table("rules")]
public class Rule
{
    [Column("id"), PrimaryKey] public Guid Id { get; set; }
    [Column("name")] public string Name { get; set; } = "";
    [Column("value")] public decimal Value { get; set; }
    [Column("start_date")] public DateOnly StartDate { get; set; }
}
