using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KdxReport.Models;

[Table("roles")]
public class Role
{
    [Key]
    [Column("role_id")]
    public int RoleId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("role_name")]
    public string RoleName { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<RoleUser> RoleUsers { get; set; } = new List<RoleUser>();
}
