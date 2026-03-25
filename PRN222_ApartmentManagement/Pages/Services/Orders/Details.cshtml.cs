using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PRN222_ApartmentManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PRN222_ApartmentManagement.Pages.Orders;

[Authorize(Roles = "BQL_Manager,BQL_Staff")]
public class DetailsModel : PageModel
{
    private readonly IServiceOrderRepository _orderRepo;
    private readonly ApartmentDbContext _context;
    private readonly bool _autoCreateInvoice;

    public DetailsModel(IServiceOrderRepository orderRepo, ApartmentDbContext context, IConfiguration config)
    {
        _orderRepo = orderRepo;
        _context = context;
        // Read setting to decide whether to auto-create invoice on completion (default: false)
        _autoCreateInvoice = config.GetValue<bool>("Billing:AutoCreateInvoiceForServiceOrder", false);
    }

    public ServiceOrder? Order { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (id <= 0) return NotFound();

        Order = await _orderRepo.GetWithDetailsAsync(id);
        if (Order == null) return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAcceptAsync(int id)
    {
        if (!User.IsInRole("BQL_Staff")) return Forbid();

        var order = await _orderRepo.GetWithDetailsAsync(id);
        if (order == null) return NotFound();

        if (order.Status != ServiceOrderStatus.Pending) return BadRequest();

        order.Status = ServiceOrderStatus.Confirmed;
        order.UpdatedAt = DateTime.Now;

        await _orderRepo.UpdateAsync(order);

        // reload order to show updated values and stay on same page
        Order = await _orderRepo.GetWithDetailsAsync(id);
        TempData["Success"] = "Đã tiếp nhận đơn.";
        return Page();
    }

    public async Task<IActionResult> OnPostCompleteAsync(int id)
    {
        var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        // only assigned staff or manager can complete
        if (!User.IsInRole("BQL_Staff") && !User.IsInRole("BQL_Manager"))
        {
            if (isAjax) return new JsonResult(new { success = false, message = "Không có quyền thực hiện hành động." }) { StatusCode = StatusCodes.Status403Forbidden };
            return Forbid();
        }

        var order = await _orderRepo.GetWithDetailsAsync(id);
        if (order == null)
        {
            if (isAjax) return new JsonResult(new { success = false, message = "Đơn không tồn tại." }) { StatusCode = StatusCodes.Status404NotFound };
            return NotFound();
        }

        if (order.Status != ServiceOrderStatus.InProgress)
        {
            if (isAjax) return new JsonResult(new { success = false, message = "Trạng thái đơn không hợp lệ." }) { StatusCode = StatusCodes.Status400BadRequest };
            return BadRequest();
        }

        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var currentUserId))
        {
            if (isAjax) return new JsonResult(new { success = false, message = "Không xác thực được người dùng." }) { StatusCode = StatusCodes.Status403Forbidden };
            return Forbid();
        }

        // allow if assigned to current user OR user is a manager
        if (order.AssignedTo.HasValue && order.AssignedTo.Value != currentUserId && !User.IsInRole("BQL_Manager"))
        {
            if (isAjax) return new JsonResult(new { success = false, message = "Bạn không được phân công cho đơn này." }) { StatusCode = StatusCodes.Status403Forbidden };
            return Forbid();
        }

        // Start transaction to update order and optionally create invoice atomically
        using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            order.Status = ServiceOrderStatus.Completed;
            order.CompletedAt = DateTime.Now;
            order.CompletedBy = currentUserId;
            order.UpdatedAt = DateTime.Now;

            await _orderRepo.UpdateAsync(order);

            int? createdInvoiceId = null;
            if (_autoCreateInvoice)
            {
                var (ok, err, invoiceId) = await CreateInvoiceForOrderAsync(order, currentUserId);
                if (!ok)
                {
                    await tx.RollbackAsync();
                    if (isAjax) return new JsonResult(new { success = false, message = err ?? "Lỗi khi tạo hóa đơn tự động." }) { StatusCode = StatusCodes.Status500InternalServerError };
                    TempData["Error"] = err ?? "Lỗi khi tạo hóa đơn tự động.";
                    return RedirectToPage(new { id });
                }

                createdInvoiceId = invoiceId;
            }

            await tx.CommitAsync();

            // reload order
            Order = await _orderRepo.GetWithDetailsAsync(id);

            // If AJAX request, return JSON so client can stay on same page and show notification
            if (isAjax)
            {
                var message = _autoCreateInvoice ? "Đã đánh dấu hoàn thành và tạo hóa đơn." : "Đã đánh dấu hoàn thành.";
                return new JsonResult(new { success = true, message = message, status = Order.Status.ToString(), invoiceId = createdInvoiceId });
            }

            // for non-AJAX, redirect back to orders list so the table reflects the updated status
            TempData["Success"] = _autoCreateInvoice ? "Đã hoàn thành đơn và tạo hóa đơn." : "Đã đánh dấu hoàn thành.";
            return Redirect("/Orders");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            if (isAjax) return new JsonResult(new { success = false, message = "Lỗi server: " + ex.Message }) { StatusCode = StatusCodes.Status500InternalServerError };
            TempData["Error"] = "Lỗi khi hoàn tất đơn: " + ex.Message;
            return RedirectToPage(new { id });
        }
    }

    // Update actual costs while InProgress
    public async Task<IActionResult> OnPostUpdateCostsAsync(int id, decimal? actualPrice, decimal? additionalCharges, string? chargeNotes)
    {
        if (!User.IsInRole("BQL_Staff") && !User.IsInRole("BQL_Manager")) return Forbid();

        var order = await _orderRepo.GetWithDetailsAsync(id);
        if (order == null) return NotFound();

        if (order.Status != ServiceOrderStatus.InProgress)
        {
            TempData["Error"] = "Chỉ có thể cập nhật chi phí khi đơn đang xử lý.";
            return RedirectToPage();
        }

        if (actualPrice.HasValue && actualPrice < 0) { TempData["Error"] = "Giá không hợp lệ."; return RedirectToPage(); }
        if (additionalCharges.HasValue && additionalCharges < 0) { TempData["Error"] = "Phí phát sinh không hợp lệ."; return RedirectToPage(); }

        order.ActualPrice = actualPrice;
        order.AdditionalCharges = additionalCharges ?? 0;
        order.ChargeNotes = string.IsNullOrWhiteSpace(chargeNotes) ? null : chargeNotes.Trim();
        order.UpdatedAt = DateTime.Now;

        await _orderRepo.UpdateAsync(order);

        TempData["Success"] = "Đã cập nhật chi phí.";
        return RedirectToPage(new { id = id });
    }

    // Create invoice for completed order (manual)
    public async Task<IActionResult> OnPostCreateInvoiceAsync(int id)
    {
        if (!User.IsInRole("BQL_Staff") && !User.IsInRole("BQL_Manager")) return Forbid();

        var order = await _orderRepo.GetWithDetailsAsync(id);
        if (order == null) return NotFound();

        if (order.Status != ServiceOrderStatus.Completed)
        {
            TempData["Error"] = "Chỉ có thể tạo hóa đơn khi đơn đã hoàn thành.";
            return RedirectToPage(new { id });
        }

        if (order.InvoiceId.HasValue)
        {
            TempData["Error"] = "Đã tồn tại hóa đơn cho đơn này.";
            return RedirectToPage(new { id });
        }

        // get current user id for CreatedBy
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var currentUserId))
        {
            return Forbid();
        }

        using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            var (ok, err, invoiceId) = await CreateInvoiceForOrderAsync(order, currentUserId);
            if (!ok)
            {
                await tx.RollbackAsync();
                TempData["Error"] = err ?? "Không thể tạo hóa đơn.";
                return RedirectToPage(new { id });
            }

            await tx.CommitAsync();

            TempData["Success"] = "Tạo hóa đơn thành công.";
            return RedirectToPage(new { id });
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            TempData["Error"] = "Lỗi khi tạo hóa đơn: " + ex.Message;
            return RedirectToPage(new { id });
        }
    }

    /// <summary>
    /// Helper to create invoice and invoice detail for a service order.
    /// Returns tuple (success, errorMessage, invoiceId)
    /// </summary>
    private async Task<(bool ok, string? error, int? invoiceId)> CreateInvoiceForOrderAsync(ServiceOrder order, int createdBy)
    {
        if (order == null) return (false, "Đơn không tồn tại.", null);
        if (order.InvoiceId.HasValue) return (false, "Đơn đã có hóa đơn.", null);

        // generate invoice number
        var invoiceNumber = await GenerateInvoiceNumberAsync();

        var issueDate = DateTime.Now.Date;
        var dueDate = issueDate.AddDays(7);

        var serviceAmount = order.ActualPrice ?? order.EstimatedPrice ?? 0m;
        var total = serviceAmount + (order.AdditionalCharges);

        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            ApartmentId = order.ApartmentId,
            BillingMonth = (order.CompletedAt ?? DateTime.Now).Month,
            BillingYear = (order.CompletedAt ?? DateTime.Now).Year,
            IssueDate = issueDate,
            DueDate = dueDate,
            TotalAmount = total,
            PaidAmount = 0m,
            Status = InvoiceStatus.Unpaid,
            CreatedBy = createdBy,
            CreatedAt = DateTime.Now
        };

        await _context.Invoices.AddAsync(invoice);
        await _context.SaveChangesAsync();

        // find latest service price for the service type
        var servicePrice = await _context.ServicePrices
            .Where(sp => sp.ServiceTypeId == order.ServiceTypeId)
            .OrderByDescending(sp => sp.EffectiveFrom)
            .FirstOrDefaultAsync();

        if (servicePrice == null)
        {
            return (false, "Không tìm thấy giá dịch vụ để tạo hóa đơn. Vui lòng thêm giá dịch vụ trước.", null);
        }

        var detail = new InvoiceDetail
        {
            InvoiceId = invoice.InvoiceId,
            ServiceTypeId = order.ServiceTypeId,
            ServicePriceId = servicePrice.ServicePriceId,
            Quantity = 1m,
            UnitPrice = serviceAmount,
            ServiceOrderId = order.ServiceOrderId,
            Description = order.Description,
            Amount = total
        };

        await _context.InvoiceDetails.AddAsync(detail);
        await _context.SaveChangesAsync();

        // link invoice to order
        order.InvoiceId = invoice.InvoiceId;
        order.UpdatedAt = DateTime.Now;
        await _orderRepo.UpdateAsync(order);

        return (true, null, invoice.InvoiceId);
    }

    private async Task<string> GenerateInvoiceNumberAsync()
    {
        var today = DateTime.Now;
        var prefix = $"INV-{today:yyyyMMdd}-";

        var last = await _context.Invoices
            .Where(i => i.InvoiceNumber.StartsWith(prefix))
            .OrderByDescending(i => i.InvoiceNumber)
            .FirstOrDefaultAsync();

        int next = 1;
        if (last != null)
        {
            var lastNumStr = last.InvoiceNumber.Replace(prefix, "");
            if (int.TryParse(lastNumStr, out var lastNum)) next = lastNum + 1;
        }

        return $"{prefix}{next:D3}";
    }
}
