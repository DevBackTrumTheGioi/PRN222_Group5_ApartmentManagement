﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("Amenities")]
public class Amenity
{
    [Key]
    public int AmenityId { get; set; }

    [Required]
    [MaxLength(100)]
    public string AmenityName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? AmenityType { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }

    public int? Capacity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PricePerHour { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual ICollection<AmenityBooking> AmenityBookings { get; set; } = new List<AmenityBooking>();
}

