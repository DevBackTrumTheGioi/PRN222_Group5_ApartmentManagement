using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Models.DTOs;

public class OperationalReportFilterDto
{
    public ReportingPeriodType PeriodType { get; set; } = ReportingPeriodType.Month;
    public int Year { get; set; } = DateTime.Now.Year;
    public int Month { get; set; } = DateTime.Now.Month;
    public int Quarter { get; set; } = ((DateTime.Now.Month - 1) / 3) + 1;
}

public class OperationalMetricComparisonDto
{
    public string Label { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string UnitSuffix { get; set; } = string.Empty;
    public decimal CurrentValue { get; set; }
    public decimal PreviousValue { get; set; }
    public decimal DeltaValue { get; set; }
    public decimal DeltaPercent { get; set; }
}

public class OperationalReportDto
{
    public string CurrentPeriodLabel { get; set; } = string.Empty;
    public string PreviousPeriodLabel { get; set; } = string.Empty;
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
    public DateTime PreviousPeriodStart { get; set; }
    public DateTime PreviousPeriodEnd { get; set; }
    public List<OperationalMetricComparisonDto> Metrics { get; set; } = new();
    public List<OperationalBreakdownItemDto> RequestStatusBreakdown { get; set; } = new();
    public List<OperationalBreakdownItemDto> ServiceOrderBreakdown { get; set; } = new();
}

public class OperationalBreakdownItemDto
{
    public string Label { get; set; } = string.Empty;
    public int CurrentCount { get; set; }
    public int PreviousCount { get; set; }
}
