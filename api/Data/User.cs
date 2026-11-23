using LinqToDB.Mapping;

namespace Data;

[Table("users")]
public class User
{
    [Column("id"), PrimaryKey] public Guid Id { get; set; }
    [Column("email")] public string Email { get; set; } = "";
    [Column("password_hash")] public string PasswordHash { get; set; } = "";
    [Column("role")] public string Role { get; set; } = "";
    [Column("active")] public bool Active { get; set; }
}
