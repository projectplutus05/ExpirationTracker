using ExpirationTracker.Data;
using ExpirationTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpirationTracker.Controllers;

public class TrucksController : Controller
{
    private readonly AppDbContext _context;
    private const long MaxDocumentBytes = 10 * 1024 * 1024;

    public TrucksController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Trucks
            .Select(t => new Truck
            {
                Id = t.Id,
                TruckNo = t.TruckNo,
                TruckMakeId = t.TruckMakeId,
                TruckMake = t.TruckMake == null ? null : new TruckMake { Name = t.TruckMake.Name },
                Model = t.Model,
                ModelYear = t.ModelYear,
                Vin = t.Vin,
                DriverId = t.DriverId,
                Driver = t.Driver == null ? null : new Driver { DriverName = t.Driver.DriverName },
                DotInspectionExpiry = t.DotInspectionExpiry,
                TruckTagExpiry = t.TruckTagExpiry,
                IrpExpiry = t.IrpExpiry,
                Documents = t.Documents
                    .Select(document => new TruckDocument
                    {
                        Id = document.Id,
                        ExpirationType = document.ExpirationType,
                        FileName = document.FileName,
                        FileSize = document.FileSize
                    })
                    .ToList()
            })
            .OrderBy(t => t.TruckNo)
            .ToListAsync());
    }

    public IActionResult Create()
    {
        PopulateDriverSelectList();
        PopulateTruckMakeSelectList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        Truck truck,
        IFormFile? dotDocument,
        IFormFile? truckTagDocument,
        IFormFile? irpDocument)
    {
        ValidateDocumentUpload(dotDocument);
        ValidateDocumentUpload(truckTagDocument);
        ValidateDocumentUpload(irpDocument);

        if (ModelState.IsValid)
        {
            _context.Add(truck);
            await _context.SaveChangesAsync();
            await SaveTruckDocumentAsync(truck.Id, "DOT", dotDocument);
            await SaveTruckDocumentAsync(truck.Id, "Tag", truckTagDocument);
            await SaveTruckDocumentAsync(truck.Id, "IRP", irpDocument);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Truck record created successfully.";
            return RedirectToAction(nameof(Index));
        }

        PopulateDriverSelectList(truck.DriverId);
        PopulateTruckMakeSelectList(truck.TruckMakeId);
        return View(truck);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var truck = await _context.Trucks
            .Select(t => new Truck
            {
                Id = t.Id,
                TruckNo = t.TruckNo,
                TruckMakeId = t.TruckMakeId,
                Model = t.Model,
                ModelYear = t.ModelYear,
                Vin = t.Vin,
                DriverId = t.DriverId,
                DotInspectionExpiry = t.DotInspectionExpiry,
                TruckTagExpiry = t.TruckTagExpiry,
                IrpExpiry = t.IrpExpiry,
                Documents = t.Documents
                    .Select(document => new TruckDocument
                    {
                        Id = document.Id,
                        ExpirationType = document.ExpirationType,
                        FileName = document.FileName,
                        FileSize = document.FileSize
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(t => t.Id == id);
        if (truck == null)
        {
            return NotFound();
        }

        PopulateDriverSelectList(truck.DriverId);
        PopulateTruckMakeSelectList(truck.TruckMakeId);
        return View(truck);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        Truck truck,
        IFormFile? dotDocument,
        IFormFile? truckTagDocument,
        IFormFile? irpDocument)
    {
        if (id != truck.Id)
        {
            return NotFound();
        }

        ValidateDocumentUpload(dotDocument);
        ValidateDocumentUpload(truckTagDocument);
        ValidateDocumentUpload(irpDocument);

        if (ModelState.IsValid)
        {
            var existingTruck = await _context.Trucks.FindAsync(id);
            if (existingTruck == null)
            {
                return NotFound();
            }

            existingTruck.DriverId = truck.DriverId;
            existingTruck.TruckNo = truck.TruckNo;
            existingTruck.TruckMakeId = truck.TruckMakeId;
            existingTruck.Model = truck.Model;
            existingTruck.ModelYear = truck.ModelYear;
            existingTruck.Vin = truck.Vin;
            existingTruck.DotInspectionExpiry = truck.DotInspectionExpiry;
            existingTruck.TruckTagExpiry = truck.TruckTagExpiry;
            existingTruck.IrpExpiry = truck.IrpExpiry;

            await SaveTruckDocumentAsync(id, "DOT", dotDocument);
            await SaveTruckDocumentAsync(id, "Tag", truckTagDocument);
            await SaveTruckDocumentAsync(id, "IRP", irpDocument);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Truck record updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        truck.Documents = await GetTruckDocumentSummaries(id);
        PopulateDriverSelectList(truck.DriverId);
        PopulateTruckMakeSelectList(truck.TruckMakeId);
        return View(truck);
    }

    public async Task<IActionResult> DownloadDocument(int id)
    {
        var document = await _context.TruckDocuments.FindAsync(id);
        if (document == null)
        {
            return NotFound();
        }

        return File(document.Content, document.ContentType, document.FileName);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDocument(int id)
    {
        var document = await _context.TruckDocuments.FindAsync(id);
        if (document == null)
        {
            return NotFound();
        }

        var truckId = document.TruckId;
        _context.TruckDocuments.Remove(document);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Truck document removed.";
        return RedirectToAction(nameof(Edit), new { id = truckId });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var truck = await _context.Trucks
            .Include(t => t.Driver)
            .Include(t => t.TruckMake)
            .FirstOrDefaultAsync(t => t.Id == id);
        if (truck == null)
        {
            return NotFound();
        }

        return View(truck);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var truck = await _context.Trucks.FindAsync(id);
        if (truck != null)
        {
            _context.Trucks.Remove(truck);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Truck record deleted.";
        }

        return RedirectToAction(nameof(Index));
    }

    private void PopulateDriverSelectList(int? selectedDriverId = null)
    {
        ViewData["DriverId"] = new SelectList(
            _context.Drivers.OrderBy(d => d.DriverName),
            nameof(Driver.Id),
            nameof(Driver.DriverName),
            selectedDriverId);
    }

    private void PopulateTruckMakeSelectList(int? selectedTruckMakeId = null)
    {
        ViewData["TruckMakeId"] = new SelectList(
            _context.TruckMakes.OrderBy(m => m.Name),
            nameof(TruckMake.Id),
            nameof(TruckMake.Name),
            selectedTruckMakeId);
    }

    private async Task SaveTruckDocumentAsync(int truckId, string expirationType, IFormFile? upload)
    {
        if (upload == null || upload.Length == 0)
        {
            return;
        }

        await using var stream = new MemoryStream();
        await upload.CopyToAsync(stream);

        var document = await _context.TruckDocuments
            .FirstOrDefaultAsync(d => d.TruckId == truckId && d.ExpirationType == expirationType);

        if (document == null)
        {
            document = new TruckDocument
            {
                TruckId = truckId,
                ExpirationType = expirationType
            };
            _context.TruckDocuments.Add(document);
        }

        document.FileName = Path.GetFileName(upload.FileName);
        document.ContentType = string.IsNullOrWhiteSpace(upload.ContentType)
            ? "application/octet-stream"
            : upload.ContentType;
        document.FileSize = upload.Length;
        document.UploadedAtUtc = DateTime.UtcNow;
        document.Content = stream.ToArray();
    }

    private void ValidateDocumentUpload(IFormFile? upload)
    {
        if (upload is { Length: > MaxDocumentBytes })
        {
            ModelState.AddModelError(string.Empty, "Documents must be 10 MB or smaller.");
        }
    }

    private Task<List<TruckDocument>> GetTruckDocumentSummaries(int truckId)
    {
        return _context.TruckDocuments
            .Where(d => d.TruckId == truckId)
            .Select(document => new TruckDocument
            {
                Id = document.Id,
                ExpirationType = document.ExpirationType,
                FileName = document.FileName,
                FileSize = document.FileSize
            })
            .ToListAsync();
    }
}
