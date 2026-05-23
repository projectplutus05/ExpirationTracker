using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpirationTracker.Data;
using ExpirationTracker.Models;

namespace ExpirationTracker.Controllers;

public class DriversController : Controller
{
    private readonly AppDbContext _context;

    public DriversController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Drivers
    public async Task<IActionResult> Index()
    {
        return View(await _context.Drivers.OrderBy(d => d.DriverName).ToListAsync());
    }

    // GET: Drivers/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Drivers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Driver driver)
    {
        if (ModelState.IsValid)
        {
            _context.Add(driver);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Driver record created successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(driver);
    }

    // GET: Drivers/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var driver = await _context.Drivers.FindAsync(id);
        if (driver == null) return NotFound();
        return View(driver);
    }

    // POST: Drivers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Driver driver)
    {
        if (id != driver.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(driver);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Driver record updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(driver);
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
}
