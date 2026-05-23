using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpirationTracker.Data;
using ExpirationTracker.Models;

namespace ExpirationTracker.Controllers;

public class TrailersController : Controller
{
    private readonly AppDbContext _context;
    private const long MaxDocumentBytes = 10 * 1024 * 1024;

    public TrailersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Trailers
    public async Task<IActionResult> Index()
    {
        return View(await _context.Trailers
            .Select(t => new Trailer
            {
                Id = t.Id,
                AssignedTo = t.AssignedTo,
                TrailerNumber = t.TrailerNumber,
                HasHeadboard = t.HasHeadboard,
                Year = t.Year,
                Make = t.Make,
                Model = t.Model,
                Vin = t.Vin,
                TagNumber = t.TagNumber,
                TagExpiry = t.TagExpiry,
                TagNotes = t.TagNotes,
                AnnualInspExpiry = t.AnnualInspExpiry,
                Documents = t.Documents
                    .Select(document => new TrailerDocument
                    {
                        Id = document.Id,
                        ExpirationType = document.ExpirationType,
                        FileName = document.FileName,
                        FileSize = document.FileSize
                    })
                    .ToList()
            })
            .OrderBy(t => t.AssignedTo)
            .ToListAsync());
    }

    // GET: Trailers/Create
    public IActionResult Create()
    {
        PopulateAssignedToSelectList();
        return View();
    }

    // POST: Trailers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Trailer trailer, IFormFile? tagDocument, IFormFile? annualInspectionDocument)
    {
        ValidateDocumentUpload(tagDocument);
        ValidateDocumentUpload(annualInspectionDocument);

        if (ModelState.IsValid)
        {
            _context.Add(trailer);
            await _context.SaveChangesAsync();
            await SaveTrailerDocumentAsync(trailer.Id, "Tag", tagDocument);
            await SaveTrailerDocumentAsync(trailer.Id, "Annual Inspection", annualInspectionDocument);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Trailer record created successfully.";
            return RedirectToAction(nameof(Index));
        }

        PopulateAssignedToSelectList(trailer.AssignedTo);
        return View(trailer);
    }

    // GET: Trailers/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var trailer = await _context.Trailers
            .Select(t => new Trailer
            {
                Id = t.Id,
                AssignedTo = t.AssignedTo,
                TrailerNumber = t.TrailerNumber,
                HasHeadboard = t.HasHeadboard,
                Year = t.Year,
                Make = t.Make,
                Model = t.Model,
                Vin = t.Vin,
                TagNumber = t.TagNumber,
                TagExpiry = t.TagExpiry,
                TagNotes = t.TagNotes,
                AnnualInspExpiry = t.AnnualInspExpiry,
                Documents = t.Documents
                    .Select(document => new TrailerDocument
                    {
                        Id = document.Id,
                        ExpirationType = document.ExpirationType,
                        FileName = document.FileName,
                        FileSize = document.FileSize
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(t => t.Id == id);
        if (trailer == null) return NotFound();

        PopulateAssignedToSelectList(trailer.AssignedTo);
        return View(trailer);
    }

    // POST: Trailers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Trailer trailer, IFormFile? tagDocument, IFormFile? annualInspectionDocument)
    {
        if (id != trailer.Id) return NotFound();

        ValidateDocumentUpload(tagDocument);
        ValidateDocumentUpload(annualInspectionDocument);

        if (ModelState.IsValid)
        {
            var existingTrailer = await _context.Trailers.FindAsync(id);
            if (existingTrailer == null) return NotFound();

            existingTrailer.AssignedTo = trailer.AssignedTo;
            existingTrailer.TrailerNumber = trailer.TrailerNumber;
            existingTrailer.HasHeadboard = trailer.HasHeadboard;
            existingTrailer.Year = trailer.Year;
            existingTrailer.Make = trailer.Make;
            existingTrailer.Model = trailer.Model;
            existingTrailer.Vin = trailer.Vin;
            existingTrailer.TagNumber = trailer.TagNumber;
            existingTrailer.TagExpiry = trailer.TagExpiry;
            existingTrailer.TagNotes = trailer.TagNotes;
            existingTrailer.AnnualInspExpiry = trailer.AnnualInspExpiry;

            await SaveTrailerDocumentAsync(id, "Tag", tagDocument);
            await SaveTrailerDocumentAsync(id, "Annual Inspection", annualInspectionDocument);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Trailer record updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        trailer.Documents = await GetTrailerDocumentSummaries(id);
        PopulateAssignedToSelectList(trailer.AssignedTo);
        return View(trailer);
    }

    public async Task<IActionResult> DownloadDocument(int id)
    {
        var document = await _context.TrailerDocuments.FindAsync(id);
        if (document == null) return NotFound();

        return File(document.Content, document.ContentType, document.FileName);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDocument(int id)
    {
        var document = await _context.TrailerDocuments.FindAsync(id);
        if (document == null) return NotFound();

        var trailerId = document.TrailerId;
        _context.TrailerDocuments.Remove(document);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Trailer document removed.";
        return RedirectToAction(nameof(Edit), new { id = trailerId });
    }

    // GET: Trailers/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var trailer = await _context.Trailers.FindAsync(id);
        if (trailer == null) return NotFound();
        return View(trailer);
    }

    // POST: Trailers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var trailer = await _context.Trailers.FindAsync(id);
        if (trailer != null)
        {
            _context.Trailers.Remove(trailer);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Trailer record deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    private void PopulateAssignedToSelectList(string? selectedAssignedTo = null)
    {
        var driverNames = _context.Drivers
            .Where(d => !string.IsNullOrWhiteSpace(d.DriverName))
            .OrderBy(d => d.DriverName)
            .Select(d => d.DriverName)
            .ToList();

        ViewData["AssignedTo"] = new SelectList(driverNames, selectedAssignedTo);
    }

    private async Task SaveTrailerDocumentAsync(int trailerId, string expirationType, IFormFile? upload)
    {
        if (upload == null || upload.Length == 0)
        {
            return;
        }

        await using var stream = new MemoryStream();
        await upload.CopyToAsync(stream);

        var document = await _context.TrailerDocuments
            .FirstOrDefaultAsync(d => d.TrailerId == trailerId && d.ExpirationType == expirationType);

        if (document == null)
        {
            document = new TrailerDocument
            {
                TrailerId = trailerId,
                ExpirationType = expirationType
            };
            _context.TrailerDocuments.Add(document);
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

    private Task<List<TrailerDocument>> GetTrailerDocumentSummaries(int trailerId)
    {
        return _context.TrailerDocuments
            .Where(d => d.TrailerId == trailerId)
            .Select(document => new TrailerDocument
            {
                Id = document.Id,
                ExpirationType = document.ExpirationType,
                FileName = document.FileName,
                FileSize = document.FileSize
            })
            .ToListAsync();
    }
}
