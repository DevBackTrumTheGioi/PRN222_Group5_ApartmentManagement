using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("RequestComments")]
public class RequestComment
{
    [Key]
    public int CommentId { get; set; }

    [Required]
    [ForeignKey("Request")]
    public int RequestId { get; set; }

    [Required]
    [ForeignKey("Author")]
    public int AuthorId { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual Request Request { get; set; } = null!;
    public virtual User Author { get; set; } = null!;
}
