﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("InvoiceDetails")]
public class InvoiceDetail
{
    [Key]
    public int InvoiceDetailId { get; set; }

    [Required]
    [ForeignKey("Invoice")]
    public int InvoiceId { get; set; }

    [Required]
    [ForeignKey("ServiceType")]
    public int ServiceTypeId { get; set; }

    [Required]
    [ForeignKey("ServicePrice")]
    public int ServicePriceId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Amount { get; set; }

    /// <summary>
    /// Reference to ServiceOrder if this invoice detail is from a service order
    /// </summary>
    [ForeignKey("ServiceOrder")]
    public int? ServiceOrderId { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;
    public virtual ServiceType ServiceType { get; set; } = null!;
    public virtual ServicePrice ServicePrice { get; set; } = null!;
    public virtual ServiceOrder? ServiceOrder { get; set; }
}

