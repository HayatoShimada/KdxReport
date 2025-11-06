using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KdxReport.Models;

[Table("read_statuses")]
public class ReadStatus
{
    [Key]
    [Column("read_status_id")]
    public int ReadStatusId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("trip_report_id")]
    public int TripReportId { get; set; }

    [Column("is_read")]
    public bool IsRead { get; set; } = false;

    [Column("read_at")]
    public DateTime? ReadAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [ForeignKey("TripReportId")]
    public TripReport TripReport { get; set; } = null!;
}
