using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("MessageReadReceipts")]
public class MessageReadReceipt
{
    [Key]
    public int ReadReceiptId { get; set; }

    [Required]
    [ForeignKey("Message")]
    public int MessageId { get; set; }

    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }

    public DateTime ReadAt { get; set; } = DateTime.Now;

    public virtual Message Message { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

