using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpirationTracker.Data;
using ExpirationTracker.Models;

namespace ExpirationTracker.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int days = 30)
    {
        var today = DateTime.Today;
        var threshold = today.AddDays(days);

        var items = new List<ExpirationItem>();

        var trucks = await _context.Trucks.ToListAsync();
        foreach (var t in trucks)
        {
            if (t.DotInspectionExpiry.HasValue)
                items.Add(MakeItem("Truck", t.TruckNo, t.TruckNo, "DOT Inspection", t.DotInspectionExpiry.Value, today, t.Id));
            if (t.TruckTagExpiry.HasValue)
                items.Add(MakeItem("Truck", t.TruckNo, t.TruckNo, "Truck Tag", t.TruckTagExpiry.Value, today, t.Id));
            if (t.IrpExpiry.HasValue)
                items.Add(MakeItem("Truck", t.TruckNo, t.TruckNo, "IRP", t.IrpExpiry.Value, today, t.Id));
        }

        var drivers = await _context.Drivers.ToListAsync();
        foreach (var d in drivers)
        {
            if (d.PhysicalExpiry.HasValue)
                items.Add(MakeItem("Driver", d.DriverName, d.DriverNumber ?? string.Empty, "Physical", d.PhysicalExpiry.Value, today, d.Id));
            if (d.LicenseExpiry.HasValue)
                items.Add(MakeItem("Driver", d.DriverName, d.DriverNumber ?? string.Empty, "License", d.LicenseExpiry.Value, today, d.Id));
        }

        var trailers = await _context.Trailers.ToListAsync();
        foreach (var t in trailers)
        {
            if (t.TagExpiry.HasValue)
                items.Add(MakeItem("Trailer", t.AssignedTo ?? "Unassigned", t.TrailerNumber, "Tag", t.TagExpiry.Value, today, t.Id));
            if (t.AnnualInspExpiry.HasValue)
                items.Add(MakeItem("Trailer", t.AssignedTo ?? "Unassigned", t.TrailerNumber, "Annual Inspection", t.AnnualInspExpiry.Value, today, t.Id));
        }

        var vm = new DashboardViewModel
        {
            DayThreshold = days,
            OverdueExpirations = items
                .Where(i => i.ExpiryDate < today)
                .OrderBy(i => i.ExpiryDate)
                .ToList(),
            UpcomingExpirations = items
                .Where(i => i.ExpiryDate >= today && i.ExpiryDate <= threshold)
                .OrderBy(i => i.ExpiryDate)
                .ToList()
        };

        return View(vm);
    }

    private static ExpirationItem MakeItem(string category, string name, string recordNumber,
        string field, DateTime expiry, DateTime today, int id)
    {
        return new ExpirationItem
        {
            Category = category,
            Name = name,
            RecordNumber = recordNumber,
            ExpirationField = field,
            ExpiryDate = expiry,
            DaysUntilExpiry = (int)(expiry - today).TotalDays,
            RecordId = id
        };
    }
}
