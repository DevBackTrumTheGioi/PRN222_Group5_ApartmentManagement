using System.Text;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class FinancialReportService : IFinancialReportService
{
    private readonly ApartmentDbContext _context;

    public FinancialReportService(ApartmentDbContext context)
    {
        _context = context;
    }

    public async Task<FinancialReportViewModel> GetReportAsync(int year, int? month)
    {
        var normalizedYear = year < 2000 || year > 3000 ? DateTime.Now.Year : year;
        var normalizedMonth = month is >= 1 and <= 12 ? month : null;
        var successStatus = (int)PaymentTransactionStatus.Success;

        var invoicesForYear = await _context.Invoices
            .AsNoTracking()
            .Include(i => i.Apartment)
            .Where(i => i.BillingYear == normalizedYear && i.Status != InvoiceStatus.Cancelled)
            .ToListAsync();

        var transactionsForYear = await _context.PaymentTransactions
            .AsNoTracking()
            .Include(t => t.Invoice)
                .ThenInclude(i => i.Apartment)
            .Include(t => t.Creator)
            .Where(t => t.PaymentDate.Year == normalizedYear && t.Status == successStatus)
            .ToListAsync();

        var filteredInvoices = normalizedMonth.HasValue
            ? invoicesForYear.Where(i => i.BillingMonth == normalizedMonth.Value).ToList()
            : invoicesForYear;

        var filteredTransactions = normalizedMonth.HasValue
            ? transactionsForYear.Where(t => t.PaymentDate.Month == normalizedMonth.Value).ToList()
            : transactionsForYear;

        var monthlyRows = BuildMonthlyRows(invoicesForYear, transactionsForYear);
        var visibleRows = normalizedMonth.HasValue
            ? monthlyRows.Where(r => r.Month == normalizedMonth.Value).ToList()
            : monthlyRows;

        var totalBilled = filteredInvoices.Sum(i => i.TotalAmount);
        var totalCollected = filteredTransactions.Sum(t => t.Amount);
        var totalOutstanding = filteredInvoices.Sum(GetRemainingAmount);
        var overdueOutstanding = filteredInvoices
            .Where(i => GetRemainingAmount(i) > 0 && i.DueDate.Date < DateTime.Now.Date)
            .Sum(GetRemainingAmount);

        var paymentMethods = filteredTransactions
            .GroupBy(t => string.IsNullOrWhiteSpace(t.PaymentMethod) ? "Khác" : t.PaymentMethod!)
            .Select(g => new PaymentMethodSummary
            {
                PaymentMethod = g.Key,
                Amount = g.Sum(t => t.Amount),
                TransactionCount = g.Count()
            })
            .OrderByDescending(x => x.Amount)
            .ToList();

        var topOutstandingApartments = filteredInvoices
            .Where(i => GetRemainingAmount(i) > 0 && i.Apartment != null)
            .GroupBy(i => new { i.ApartmentId, i.Apartment.ApartmentNumber, i.Apartment.BuildingBlock })
            .Select(g => new ApartmentDebtSummary
            {
                ApartmentId = g.Key.ApartmentId,
                ApartmentNumber = g.Key.ApartmentNumber,
                BuildingBlock = g.Key.BuildingBlock ?? "-",
                InvoiceCount = g.Count(),
                OutstandingAmount = g.Sum(GetRemainingAmount)
            })
            .OrderByDescending(x => x.OutstandingAmount)
            .Take(5)
            .ToList();

        var recentTransactions = filteredTransactions
            .OrderByDescending(t => t.PaymentDate)
            .Take(8)
            .ToList();

        return new FinancialReportViewModel
        {
            Year = normalizedYear,
            Month = normalizedMonth,
            PeriodLabel = normalizedMonth.HasValue ? $"Tháng {normalizedMonth.Value:D2}/{normalizedYear}" : $"Năm {normalizedYear}",
            TotalBilled = totalBilled,
            TotalCollected = totalCollected,
            TotalOutstanding = totalOutstanding,
            OverdueOutstanding = overdueOutstanding,
            TotalInvoices = filteredInvoices.Count,
            PaidInvoices = filteredInvoices.Count(i => i.Status == InvoiceStatus.Paid),
            SuccessfulTransactions = filteredTransactions.Count,
            CollectionRate = totalBilled > 0 ? Math.Round(totalCollected / totalBilled * 100m, 2) : 0,
            MonthlyTrend = monthlyRows.Select(r => new FinancialTrendPoint
            {
                Month = r.Month,
                Label = r.Label,
                BilledAmount = r.BilledAmount,
                CollectedAmount = r.CollectedAmount
            }).ToList(),
            MonthlyRows = visibleRows,
            PaymentMethods = paymentMethods,
            TopOutstandingApartments = topOutstandingApartments,
            RecentTransactions = recentTransactions
        };
    }

    public async Task<byte[]> ExportExcelAsync(int year, int? month)
    {
        var report = await GetReportAsync(year, month);
        var builder = new StringBuilder();

        builder.AppendLine("<html><head><meta charset='utf-8' /></head><body>");
        builder.AppendLine($"<h2>Báo cáo tài chính {report.PeriodLabel}</h2>");
        builder.AppendLine("<table border='1' cellspacing='0' cellpadding='4'>");
        builder.AppendLine("<tr><th>Chỉ số</th><th>Giá trị</th></tr>");
        builder.AppendLine($"<tr><td>Tổng phát hành</td><td>{report.TotalBilled:N0}</td></tr>");
        builder.AppendLine($"<tr><td>Tổng thu</td><td>{report.TotalCollected:N0}</td></tr>");
        builder.AppendLine($"<tr><td>Công nợ</td><td>{report.TotalOutstanding:N0}</td></tr>");
        builder.AppendLine($"<tr><td>Công nợ quá hạn</td><td>{report.OverdueOutstanding:N0}</td></tr>");
        builder.AppendLine($"<tr><td>Tỷ lệ thu</td><td>{report.CollectionRate:N2}%</td></tr>");
        builder.AppendLine("</table>");

        builder.AppendLine("<br/><h3>Chi tiết theo tháng</h3>");
        builder.AppendLine("<table border='1' cellspacing='0' cellpadding='4'>");
        builder.AppendLine("<tr><th>Tháng</th><th>Phát hành</th><th>Đã thu</th><th>Công nợ</th><th>Số hóa đơn</th><th>Đã thanh toán đủ</th><th>Số giao dịch</th></tr>");
        foreach (var row in report.MonthlyRows)
        {
            builder.AppendLine(
                $"<tr><td>{row.Label}</td><td>{row.BilledAmount:N0}</td><td>{row.CollectedAmount:N0}</td><td>{row.OutstandingAmount:N0}</td><td>{row.InvoiceCount}</td><td>{row.PaidInvoiceCount}</td><td>{row.SuccessfulTransactionCount}</td></tr>");
        }
        builder.AppendLine("</table>");

        builder.AppendLine("<br/><h3>Cơ cấu phương thức thanh toán</h3>");
        builder.AppendLine("<table border='1' cellspacing='0' cellpadding='4'>");
        builder.AppendLine("<tr><th>Phương thức</th><th>Số giao dịch</th><th>Số tiền</th></tr>");
        foreach (var item in report.PaymentMethods)
        {
            builder.AppendLine($"<tr><td>{item.PaymentMethod}</td><td>{item.TransactionCount}</td><td>{item.Amount:N0}</td></tr>");
        }
        builder.AppendLine("</table>");

        builder.AppendLine("</body></html>");
        return Encoding.UTF8.GetBytes(builder.ToString());
    }

    private static List<FinancialMonthlyRow> BuildMonthlyRows(IEnumerable<Invoice> invoices, IEnumerable<PaymentTransaction> transactions)
    {
        var result = new List<FinancialMonthlyRow>();

        for (var month = 1; month <= 12; month++)
        {
            var invoicesInMonth = invoices.Where(i => i.BillingMonth == month).ToList();
            var transactionsInMonth = transactions.Where(t => t.PaymentDate.Month == month).ToList();

            result.Add(new FinancialMonthlyRow
            {
                Month = month,
                Label = $"Tháng {month:D2}",
                BilledAmount = invoicesInMonth.Sum(i => i.TotalAmount),
                CollectedAmount = transactionsInMonth.Sum(t => t.Amount),
                OutstandingAmount = invoicesInMonth.Sum(GetRemainingAmount),
                InvoiceCount = invoicesInMonth.Count,
                PaidInvoiceCount = invoicesInMonth.Count(i => i.Status == InvoiceStatus.Paid),
                SuccessfulTransactionCount = transactionsInMonth.Count
            });
        }

        return result;
    }

    private static decimal GetRemainingAmount(Invoice invoice)
    {
        var remaining = invoice.TotalAmount - invoice.PaidAmount;
        return remaining > 0 ? remaining : 0;
    }
}
