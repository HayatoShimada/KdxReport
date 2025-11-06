using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KdxReport.Models;

[Table("equipment")]
public class Equipment
{
    [Key]
    [Column("equipment_id")]
    public int EquipmentId { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("equipment_name")]
    public string EquipmentName { get; set; } = string.Empty;

    [Column("total_counter")]
    public long? TotalCounter { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<TripReport> TripReports { get; set; } = new List<TripReport>();
    public ICollection<Thread> Threads { get; set; } = new List<Thread>();
}
