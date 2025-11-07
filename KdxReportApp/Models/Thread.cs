using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KdxReport.Models;

[Table("threads")]
public class Thread
{
    [Key]
    [Column("thread_id")]
    public int ThreadId { get; set; }

    /// <summary>
    /// 外部DB（MstCompany）の会社コード
    /// </summary>
    [MaxLength(50)]
    [Column("company_cd")]
    public string CompanyCd { get; set; } = string.Empty;

    [Column("equipment_id")]
    public int? EquipmentId { get; set; }

    [Column("trip_report_id")]
    public int? TripReportId { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("thread_name")]
    public string ThreadName { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("EquipmentId")]
    public Equipment? Equipment { get; set; }

    [ForeignKey("TripReportId")]
    public TripReport? TripReport { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
