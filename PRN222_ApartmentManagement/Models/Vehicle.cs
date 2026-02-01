using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("Vehicles")]
public class Vehicle
{
    [Key]
    public int VehicleId { get; set; }

    [Required]
    [ForeignKey("Resident")]
    public int ResidentId { get; set; }

    [MaxLength(50)]
    public string? VehicleType { get; set; }

    [MaxLength(20)]
    public string? LicensePlate { get; set; }

    [MaxLength(50)]
    public string? Brand { get; set; }

    [MaxLength(50)]
    public string? Model { get; set; }

    [MaxLength(30)]
    public string? Color { get; set; }

    [Column(TypeName = "date")]
    public DateTime? RegisteredDate { get; set; }

    public bool IsActive { get; set; } = true;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual Resident Resident { get; set; } = null!;
    public virtual ICollection<ResidentCard> ResidentCards { get; set; } = new List<ResidentCard>();
}

