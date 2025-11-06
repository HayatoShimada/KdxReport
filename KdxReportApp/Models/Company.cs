using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KdxReport.Models;

[Table("companies")]
public class Company
{
    [Key]
    [Column("company_id")]
    public int CompanyId { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("company_name")]
    public string CompanyName { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<CompanyContact> CompanyContacts { get; set; } = new List<CompanyContact>();
    public ICollection<TripReport> TripReports { get; set; } = new List<TripReport>();
    public ICollection<Thread> Threads { get; set; } = new List<Thread>();
}
