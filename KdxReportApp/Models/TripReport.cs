using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KdxReport.Models;

[Table("trip_reports")]
public class TripReport
{
    [Key]
    [Column("trip_report_id")]
    public int TripReportId { get; set; }

    [Column("company_id")]
    public int CompanyId { get; set; }

    [Column("contact_id")]
    public int ContactId { get; set; }

    [Column("equipment_id")]
    public int EquipmentId { get; set; }

    [Required]
    [Column("trip_start_date")]
    public DateTime TripStartDate { get; set; }

    [Required]
    [Column("trip_end_date")]
    public DateTime TripEndDate { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("submitter")]
    public string Submitter { get; set; } = string.Empty;

    [MaxLength(500)]
    [Column("companions")]
    public string? Companions { get; set; }

    [Column("content")]
    public string Content { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("approval_status")]
    public string ApprovalStatus { get; set; } = "pending"; // pending, approved, rejected

    [Column("approved_by")]
    public int? ApprovedBy { get; set; }

    [Column("approved_at")]
    public DateTime? ApprovedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("CompanyId")]
    public Company Company { get; set; } = null!;

    [ForeignKey("ContactId")]
    public CompanyContact Contact { get; set; } = null!;

    [ForeignKey("EquipmentId")]
    public Equipment Equipment { get; set; } = null!;

    [ForeignKey("ApprovedBy")]
    public User? Approver { get; set; }

    public ICollection<Models.Thread> Threads { get; set; } = new List<Models.Thread>();
    public ICollection<ReadStatus> ReadStatuses { get; set; } = new List<ReadStatus>();
}
