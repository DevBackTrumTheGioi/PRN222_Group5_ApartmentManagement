using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Models;

[Table("ResidentCards")]
public class ResidentCard
{
    [Key]
    public int ResidentCardId { get; set; }

    [Required]
    [MaxLength(50)]
    public string CardNumber { get; set; } = string.Empty;

    public CardType? CardType { get; set; }

    [Required]
    [ForeignKey("User")]
    public int ResidentId { get; set; }

    [ForeignKey("Vehicle")]
    public int? VehicleId { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateTime IssuedDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? ExpiryDate { get; set; }

    public CardStatus Status { get; set; } = CardStatus.Active;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public virtual User Resident { get; set; } = null!;
    public virtual Vehicle? Vehicle { get; set; }
}
