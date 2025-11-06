using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KdxReport.Models;

[Table("company_contacts")]
public class CompanyContact
{
    [Key]
    [Column("contact_id")]
    public int ContactId { get; set; }

    [Column("company_id")]
    public int CompanyId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("contact_name")]
    public string ContactName { get; set; } = string.Empty;

    [MaxLength(255)]
    [Column("email")]
    public string? Email { get; set; }

    [MaxLength(50)]
    [Column("phone")]
    public string? Phone { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("CompanyId")]
    public Company Company { get; set; } = null!;

    public ICollection<TripReport> TripReports { get; set; } = new List<TripReport>();
}
