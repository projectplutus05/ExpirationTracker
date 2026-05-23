namespace ExpirationTracker.Models;

public class DashboardViewModel
{
    public List<ExpirationItem> UpcomingExpirations { get; set; } = new();
    public List<ExpirationItem> OverdueExpirations { get; set; } = new();
    public int DayThreshold { get; set; } = 30;
}

public class ExpirationItem
{
    public string Category { get; set; } = string.Empty;   // "Driver" or "Trailer"
    public string Name { get; set; } = string.Empty;
    public string RecordNumber { get; set; } = string.Empty;
    public string ExpirationField { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int DaysUntilExpiry { get; set; }
    public int RecordId { get; set; }
}
