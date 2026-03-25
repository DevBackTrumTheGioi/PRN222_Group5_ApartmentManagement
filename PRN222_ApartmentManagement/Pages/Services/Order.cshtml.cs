using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Pages.Services;

[Authorize(Policy = "ResidentOnly")]
public class OrderModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public OrderModel(ApartmentDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public int? ServiceTypeId { get; set; }

    public ServiceType? ServiceType { get; set; }

    public List<ServiceType> ServiceTypes { get; set; } = new();

    public List<ServicePrice> AvailablePrices { get; set; } = new();

    [BindProperty]
    public OrderInput Input { get; set; } = new();

    public class OrderInput
    {
        // Use Range to validate positive id
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn dịch vụ")]
        public int ServiceTypeId { get; set; }

        // Selected price id
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn mức giá")]
        public int ServicePriceId { get; set; }

        [DataType(DataType.Date)]
        public DateTime RequestedDate { get; set; } = DateTime.Today;
        public string RequestedTimeSlot { get; set; } = "Morning";
        public string? Description { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        ServiceTypes = await _context.ServiceTypes
            .Where(st => st.IsActive && !st.IsDeleted)
            .OrderBy(st => st.ServiceTypeName)
            .ToListAsync();

        if (ServiceTypeId.HasValue)
        {
            Input.ServiceTypeId = ServiceTypeId.Value;
            ServiceType = ServiceTypes.FirstOrDefault(s => s.ServiceTypeId == ServiceTypeId.Value);

            if (ServiceType != null)
            {
                AvailablePrices = await _context.ServicePrices
                    .Where(sp => sp.ServiceTypeId == ServiceType.ServiceTypeId)
                    .OrderByDescending(sp => sp.EffectiveFrom)
                    .ToListAsync();

                if (AvailablePrices.Any())
                {
                    // default to latest price
                    Input.ServicePriceId = AvailablePrices.First().ServicePriceId;
                }
            }
        }

        return Page();
    }

    // JSON endpoint for fetching prices by serviceTypeId from client-side
    public async Task<JsonResult> OnGetPricesAsync(int serviceTypeId)
    {
        var prices = await _context.ServicePrices
            .Where(sp => sp.ServiceTypeId == serviceTypeId)
            .OrderByDescending(sp => sp.EffectiveFrom)
            .Select(sp => new {
                sp.ServicePriceId,
                UnitPrice = sp.UnitPrice,
                EffectiveFrom = sp.EffectiveFrom
            })
            .ToListAsync();

        return new JsonResult(prices);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // reload service list and prices for redisplay in case of validation errors
        ServiceTypes = await _context.ServiceTypes
            .Where(st => st.IsActive && !st.IsDeleted)
            .OrderBy(st => st.ServiceTypeName)
            .ToListAsync();

        AvailablePrices = await _context.ServicePrices
            .Where(sp => sp.ServiceTypeId == Input.ServiceTypeId)
            .OrderByDescending(sp => sp.EffectiveFrom)
            .ToListAsync();

        // Validate model
        if (!ModelState.IsValid)
        {
            ServiceType = ServiceTypes.FirstOrDefault(s => s.ServiceTypeId == Input.ServiceTypeId);
            return Page();
        }

        // Validate the selected service type exists
        var selectedServiceType = await _context.ServiceTypes.FindAsync(Input.ServiceTypeId);
        if (selectedServiceType == null)
        {
            ModelState.AddModelError("Input.ServiceTypeId", "Dịch vụ được chọn không tồn tại.");
            ServiceType = null;
            return Page();
        }

        // Validate selected price
        var selectedPrice = await _context.ServicePrices.FindAsync(Input.ServicePriceId);
        if (selectedPrice == null || selectedPrice.ServiceTypeId != Input.ServiceTypeId)
        {
            ModelState.AddModelError("Input.ServicePriceId", "Mức giá không hợp lệ cho dịch vụ này.");
            ServiceType = selectedServiceType;
            return Page();
        }

        // Validate requested date is not in the past
        if (Input.RequestedDate.Date < DateTime.Today)
        {
            ModelState.AddModelError("Input.RequestedDate", "Ngày yêu cầu phải là ngày hôm nay hoặc ngày trong tương lai.");
            ServiceType = selectedServiceType;
            return Page();
        }

        // Get current user
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Forbid();
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return Forbid();

        var apartmentId = user.ApartmentId ?? 0;

        // Generate order number
        string orderNumberPrefix = $"SO-{DateTime.Now:yyyyMMdd}-";
        var last = await _context.ServiceOrders.Where(so => so.OrderNumber.StartsWith(orderNumberPrefix)).OrderByDescending(so => so.OrderNumber).FirstOrDefaultAsync();
        int next = 1;
        if (last != null)
        {
            var lastNum = last.OrderNumber.Replace(orderNumberPrefix, "");
            if (int.TryParse(lastNum, out var ln)) next = ln + 1;
        }

        // use quantity = 1 by default
        decimal unitPrice = selectedPrice.UnitPrice;
        decimal estimated = unitPrice * 1m;

        var order = new ServiceOrder
        {
            OrderNumber = orderNumberPrefix + next.ToString("D3"),
            ApartmentId = apartmentId,
            ResidentId = user.UserId,
            ServiceTypeId = Input.ServiceTypeId,
            RequestedDate = Input.RequestedDate.Date,
            RequestedTimeSlot = Input.RequestedTimeSlot,
            Description = Input.Description,
            Status = ServiceOrderStatus.Pending,
            CreatedAt = DateTime.Now,
            EstimatedPrice = estimated
        };

        _context.ServiceOrders.Add(order);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Services/MyOrders");
    }
}
