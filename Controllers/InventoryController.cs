using ManufacturingTimeTracking.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManufacturingTimeTracking.Controllers;

[Authorize(Policy = "CanUseInventory")]
public class InventoryController : Controller
{
    private readonly ApplicationDbContext _db;

    public InventoryController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Stock()
    {
        var rows = await _db.StockSnapshots
            .Include(s => s.Item)
            .Include(s => s.Location)
            .OrderByDescending(s => s.UpdatedAtUtc)
            .Take(500)
            .ToListAsync();
        return View(rows);
    }

    public async Task<IActionResult> Movements()
    {
        var rows = await _db.Movements
            .Include(m => m.Item)
            .Include(m => m.FromLocation)
            .Include(m => m.ToLocation)
            .OrderByDescending(m => m.TimestampUtc)
            .Take(500)
            .ToListAsync();
        return View(rows);
    }
}
