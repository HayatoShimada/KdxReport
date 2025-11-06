using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KdxReport.Models;

[Table("role_users")]
public class RoleUser
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("RoleId")]
    public Role Role { get; set; } = null!;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}
