using ExpirationTracker.Data;
using ExpirationTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpirationTracker.Controllers;

public class TrucksController : Controller
{
    private readonly AppDbContext _context;

    public TrucksController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Trucks
            .Include(t => t.Driver)
            .Include(t => t.TruckMake)
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
    public async Task<IActionResult> Create(Truck truck)
    {
        if (ModelState.IsValid)
        {
            _context.Add(truck);
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
        var truck = await _context.Trucks.FindAsync(id);
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
    public async Task<IActionResult> Edit(int id, Truck truck)
    {
        if (id != truck.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _context.Update(truck);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Truck record updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        PopulateDriverSelectList(truck.DriverId);
        PopulateTruckMakeSelectList(truck.TruckMakeId);
        return View(truck);
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
}
