using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

[Table("PaymentTransactions")]
public class PaymentTransaction
{
    [Key]
    public int TransactionId { get; set; }

    [Required]
    [ForeignKey("Invoice")]
    public int InvoiceId { get; set; }

    [Required]
    [MaxLength(100)]
    public string TransactionCode { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(50)]
    public string? PaymentMethod { get; set; }

    [Required]
    public DateTime PaymentDate { get; set; }

    public int Status { get; set; } = 0;

    public string? GatewayResponse { get; set; }

    [MaxLength(100)]
    public string? VnpTxnRef { get; set; }

    [MaxLength(50)]
    public string? VnpTransactionNo { get; set; }

    [MaxLength(10)]
    public string? VnpResponseCode { get; set; }

    [MaxLength(20)]
    public string? VnpBankCode { get; set; }

    [MaxLength(30)]
    public string? VnpPayDate { get; set; }

    [ForeignKey("Creator")]
    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual Invoice Invoice { get; set; } = null!;
    public virtual User? Creator { get; set; }
}

