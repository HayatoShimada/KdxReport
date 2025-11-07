using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KdxReport.Models;

[Table("trip_reports")]
public class TripReport
{
    [Key]
    [Column("trip_report_id")]
    public int TripReportId { get; set; }

    /// <summary>
    /// KPRO（MstCompany）の会社コード
    /// </summary>
    [MaxLength(50)]
    [Column("company_cd")]
    public string CompanyCd { get; set; } = string.Empty;

    /// <summary>
    /// KPRO（MstCustomerContact）の得意先コード
    /// </summary>
    [MaxLength(50)]
    [Column("customer_cd")]
    public string CustomerCd { get; set; } = string.Empty;

    /// <summary>
    /// KPRO（MstCustomerContact）の担当者コード
    /// </summary>
    [MaxLength(50)]
    [Column("staff_cd")]
    public string StaffCd { get; set; } = string.Empty;

    [Column("equipment_id")]
    public int EquipmentId { get; set; }

    [Required]
    [Column("trip_start_date")]
    public DateTime TripStartDate { get; set; }

    [Required]
    [Column("trip_end_date")]
    public DateTime TripEndDate { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

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
    [ForeignKey("EquipmentId")]
    public Equipment Equipment { get; set; } = null!;

    [ForeignKey("ApprovedBy")]
    public User? Approver { get; set; }

    public ICollection<Models.Thread> Threads { get; set; } = new List<Models.Thread>();
    public ICollection<ReadStatus> ReadStatuses { get; set; } = new List<ReadStatus>();
}
