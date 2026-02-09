﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

/// <summary>
/// Resident entity representing apartment residents
/// </summary>
[Table("Residents")]
public class Resident : User
{
    public DateTime? DateOfBirth { get; set; }

    [MaxLength(20)]
    public string? IdentityCardNumber { get; set; }

    [MaxLength(50)]
    public string? ResidentType { get; set; }

    [MaxLength(20)]
    public string? ResidencyStatus { get; set; }

    [ForeignKey("Apartment")]
    public int? ApartmentId { get; set; }

    public DateTime? MoveInDate { get; set; }

    public DateTime? MoveOutDate { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }


    // Navigation properties
    public virtual Apartment? Apartment { get; set; }
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    public virtual ICollection<ResidentCard> ResidentCards { get; set; } = new List<ResidentCard>();
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
    public virtual ICollection<Visitor> RegisteredVisitors { get; set; } = new List<Visitor>();
    public virtual ICollection<Parcel> PickedUpParcels { get; set; } = new List<Parcel>();
    public virtual ICollection<AmenityBooking> AmenityBookings { get; set; } = new List<AmenityBooking>();
    public virtual ICollection<ContractMember> ContractMembers { get; set; } = new List<ContractMember>();
}

