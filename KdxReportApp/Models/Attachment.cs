using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KdxReport.Models;

[Table("attachments")]
public class Attachment
{
    [Key]
    [Column("attachment_id")]
    public int AttachmentId { get; set; }

    [Column("thread_id")]
    public int ThreadId { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("file_name")]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    [Column("file_path")]
    public string FilePath { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("file_type")]
    public string FileType { get; set; } = string.Empty;

    [Column("file_size")]
    public long FileSize { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("ThreadId")]
    public Thread Thread { get; set; } = null!;
}
