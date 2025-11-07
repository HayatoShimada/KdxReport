using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KdxReport.Models;

[Table("users")]
public class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("user_name")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("password")]
    public string Password { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// MstStaffの登録番号（SerialNo）との紐づけ
    /// </summary>
    [MaxLength(50)]
    [Column("staff_serial_no")]
    public string? StaffSerialNo { get; set; }

    // Navigation properties
    public ICollection<RoleUser> RoleUsers { get; set; } = new List<RoleUser>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<ReadStatus> ReadStatuses { get; set; } = new List<ReadStatus>();
    public ICollection<TripReport> ApprovedTripReports { get; set; } = new List<TripReport>();
}
