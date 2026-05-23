using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpirationTracker.Data;
using ExpirationTracker.Models;

namespace ExpirationTracker.Controllers;

public class TrailersController : Controller
{
    private readonly AppDbContext _context;

    public TrailersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Trailers
    public async Task<IActionResult> Index()
    {
        return View(await _context.Trailers.OrderBy(t => t.AssignedTo).ToListAsync());
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
    public async Task<IActionResult> Create(Trailer trailer)
    {
        if (ModelState.IsValid)
        {
            _context.Add(trailer);
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
        var trailer = await _context.Trailers.FindAsync(id);
        if (trailer == null) return NotFound();

        PopulateAssignedToSelectList(trailer.AssignedTo);
        return View(trailer);
    }

    // POST: Trailers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Trailer trailer)
    {
        if (id != trailer.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(trailer);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Trailer record updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        PopulateAssignedToSelectList(trailer.AssignedTo);
        return View(trailer);
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
}
