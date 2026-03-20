using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("UserRefreshTokens")]
public class UserRefreshToken
{
    [Key]
    public int RefreshTokenId { get; set; }

    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }

    [Required]
    [MaxLength(255)]
    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? RevokedAt { get; set; }

    [MaxLength(255)]
    public string? ReplacedByTokenHash { get; set; }

    [MaxLength(50)]
    public string? CreatedByIp { get; set; }

    [MaxLength(50)]
    public string? RevokedByIp { get; set; }

    [MaxLength(250)]
    public string? RevocationReason { get; set; }

    public virtual User User { get; set; } = null!;
}
