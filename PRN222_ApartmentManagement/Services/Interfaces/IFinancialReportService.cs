using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface IFinancialReportService
{
    Task<FinancialReportViewModel> GetReportAsync(int year, int? month);
    Task<byte[]> ExportExcelAsync(int year, int? month);
}

public class FinancialReportViewModel
{
    public int Year { get; set; }
    public int? Month { get; set; }
    public string PeriodLabel { get; set; } = string.Empty;
    public decimal TotalBilled { get; set; }
    public decimal TotalCollected { get; set; }
    public decimal TotalOutstanding { get; set; }
    public decimal OverdueOutstanding { get; set; }
    public int TotalInvoices { get; set; }
    public int PaidInvoices { get; set; }
    public int SuccessfulTransactions { get; set; }
    public decimal CollectionRate { get; set; }
    public List<FinancialTrendPoint> MonthlyTrend { get; set; } = new();
    public List<FinancialMonthlyRow> MonthlyRows { get; set; } = new();
    public List<PaymentMethodSummary> PaymentMethods { get; set; } = new();
    public List<ApartmentDebtSummary> TopOutstandingApartments { get; set; } = new();
    public List<PaymentTransaction> RecentTransactions { get; set; } = new();
}

public class FinancialTrendPoint
{
    public int Month { get; set; }
    public string Label { get; set; } = string.Empty;
    public decimal BilledAmount { get; set; }
    public decimal CollectedAmount { get; set; }
}

public class FinancialMonthlyRow
{
    public int Month { get; set; }
    public string Label { get; set; } = string.Empty;
    public decimal BilledAmount { get; set; }
    public decimal CollectedAmount { get; set; }
    public decimal OutstandingAmount { get; set; }
    public int InvoiceCount { get; set; }
    public int PaidInvoiceCount { get; set; }
    public int SuccessfulTransactionCount { get; set; }
}

public class PaymentMethodSummary
{
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int TransactionCount { get; set; }
}

public class ApartmentDebtSummary
{
    public int ApartmentId { get; set; }
    public string ApartmentNumber { get; set; } = string.Empty;
    public string BuildingBlock { get; set; } = string.Empty;
    public int InvoiceCount { get; set; }
    public decimal OutstandingAmount { get; set; }
}
