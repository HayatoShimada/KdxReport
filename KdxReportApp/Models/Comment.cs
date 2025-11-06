using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KdxReport.Models;

[Table("comments")]
public class Comment
{
    [Key]
    [Column("comment_id")]
    public int CommentId { get; set; }

    [Column("thread_id")]
    public int ThreadId { get; set; }

    [Column("parent_comment_id")]
    public int? ParentCommentId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Required]
    [Column("comment_content")]
    public string CommentContent { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("ThreadId")]
    public Thread Thread { get; set; } = null!;

    [ForeignKey("ParentCommentId")]
    public Comment? ParentComment { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}
