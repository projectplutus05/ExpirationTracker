using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpirationTracker.Data;
using ExpirationTracker.Models;

namespace ExpirationTracker.Controllers;

public class DriversController : Controller
{
    private readonly AppDbContext _context;
    private const long MaxDocumentBytes = 10 * 1024 * 1024;

    public DriversController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Drivers
    public async Task<IActionResult> Index()
    {
        return View(await _context.Drivers
            .Select(d => new Driver
            {
                Id = d.Id,
                DriverNumber = d.DriverNumber,
                DriverName = d.DriverName,
                PhysicalExpiry = d.PhysicalExpiry,
                LicenseExpiry = d.LicenseExpiry,
                Documents = d.Documents
                    .Select(document => new DriverDocument
                    {
                        Id = document.Id,
                        ExpirationType = document.ExpirationType,
                        FileName = document.FileName,
                        FileSize = document.FileSize
                    })
                    .ToList()
            })
            .OrderBy(d => d.DriverName)
            .ToListAsync());
    }

    // GET: Drivers/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Drivers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Driver driver, IFormFile? physicalDocument, IFormFile? licenseDocument)
    {
        ValidateDocumentUpload(physicalDocument);
        ValidateDocumentUpload(licenseDocument);

        if (ModelState.IsValid)
        {
            _context.Add(driver);
            await _context.SaveChangesAsync();
            await SaveDriverDocumentAsync(driver.Id, "Physical", physicalDocument);
            await SaveDriverDocumentAsync(driver.Id, "License", licenseDocument);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Driver record created successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(driver);
    }

    // GET: Drivers/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var driver = await _context.Drivers
            .Select(d => new Driver
            {
                Id = d.Id,
                DriverNumber = d.DriverNumber,
                DriverName = d.DriverName,
                PhysicalExpiry = d.PhysicalExpiry,
                LicenseExpiry = d.LicenseExpiry,
                Documents = d.Documents
                    .Select(document => new DriverDocument
                    {
                        Id = document.Id,
                        ExpirationType = document.ExpirationType,
                        FileName = document.FileName,
                        FileSize = document.FileSize
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(d => d.Id == id);
        if (driver == null) return NotFound();
        return View(driver);
    }

    // POST: Drivers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Driver driver, IFormFile? physicalDocument, IFormFile? licenseDocument)
    {
        if (id != driver.Id) return NotFound();

        ValidateDocumentUpload(physicalDocument);
        ValidateDocumentUpload(licenseDocument);

        if (ModelState.IsValid)
        {
            var existingDriver = await _context.Drivers.FindAsync(id);
            if (existingDriver == null) return NotFound();

            existingDriver.DriverNumber = driver.DriverNumber;
            existingDriver.DriverName = driver.DriverName;
            existingDriver.PhysicalExpiry = driver.PhysicalExpiry;
            existingDriver.LicenseExpiry = driver.LicenseExpiry;

            await SaveDriverDocumentAsync(id, "Physical", physicalDocument);
            await SaveDriverDocumentAsync(id, "License", licenseDocument);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Driver record updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        driver.Documents = await _context.DriverDocuments
            .Where(d => d.DriverId == id)
            .Select(document => new DriverDocument
            {
                Id = document.Id,
                ExpirationType = document.ExpirationType,
                FileName = document.FileName,
                FileSize = document.FileSize
            })
            .ToListAsync();
        return View(driver);
    }

    public async Task<IActionResult> DownloadDocument(int id)
    {
        var document = await _context.DriverDocuments.FindAsync(id);
        if (document == null) return NotFound();

        return File(document.Content, document.ContentType, document.FileName);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDocument(int id)
    {
        var document = await _context.DriverDocuments.FindAsync(id);
        if (document == null) return NotFound();

        var driverId = document.DriverId;
        _context.DriverDocuments.Remove(document);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Driver document removed.";
        return RedirectToAction(nameof(Edit), new { id = driverId });
    }

    // GET: Drivers/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var driver = await _context.Drivers.FindAsync(id);
        if (driver == null) return NotFound();
        return View(driver);
    }

    // POST: Drivers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var driver = await _context.Drivers.FindAsync(id);
        if (driver != null)
        {
            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Driver record deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task SaveDriverDocumentAsync(int driverId, string expirationType, IFormFile? upload)
    {
        if (upload == null || upload.Length == 0)
        {
            return;
        }

        await using var stream = new MemoryStream();
        await upload.CopyToAsync(stream);

        var document = await _context.DriverDocuments
            .FirstOrDefaultAsync(d => d.DriverId == driverId && d.ExpirationType == expirationType);

        if (document == null)
        {
            document = new DriverDocument
            {
                DriverId = driverId,
                ExpirationType = expirationType
            };
            _context.DriverDocuments.Add(document);
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
}
