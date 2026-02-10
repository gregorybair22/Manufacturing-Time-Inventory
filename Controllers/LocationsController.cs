using System.Text;
using ManufacturingTimeTracking.Data;
using ManufacturingTimeTracking.Models.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace ManufacturingTimeTracking.Controllers;

[Authorize(Policy = "CanUseInventory")]
public class LocationsController : Controller
{
    private readonly ApplicationDbContext _db;

    public LocationsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Details(int id)
    {
        var loc = await _db.Locations.FindAsync(id);
        if (loc == null) return NotFound();
        return View(loc);
    }

    public async Task<IActionResult> Qr(int id, int pixelsPerModule = 10)
    {
        var loc = await _db.Locations.FindAsync(id);
        if (loc == null) return NotFound();
        var code = loc.Code ?? "";
        if (string.IsNullOrWhiteSpace(code)) return NotFound();
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
        var png = new PngByteQRCode(data).GetGraphic(pixelsPerModule);
        return File(png, "image/png");
    }

    [HttpGet]
    public async Task<IActionResult> PrintLabel(int id)
    {
        var loc = await _db.Locations.FindAsync(id);
        if (loc == null) return NotFound();
        return View(loc);
    }

    public async Task<IActionResult> Index(string? zone = null)
    {
        var q = _db.Locations.AsQueryable();
        if (!string.IsNullOrWhiteSpace(zone))
            q = q.Where(l => l.Zone == zone);
        ViewBag.Zones = await _db.Locations.Select(l => l.Zone).Distinct().OrderBy(z => z).ToListAsync();
        ViewBag.Zone = zone ?? "";
        var rows = await q.OrderBy(l => l.Zone).ThenBy(l => l.X).ThenBy(l => l.Y).ThenBy(l => l.Z).Take(2000).ToListAsync();
        return View(rows);
    }

    public IActionResult Create() => View(new Location { Zone = "Z1", Type = "Shelf", CapacityUnits = 100 });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Location model)
    {
        if (string.IsNullOrWhiteSpace(model.Code))
            ModelState.AddModelError(nameof(Location.Code), "Location code is required.");
        if (!ModelState.IsValid) return View(model);
        model.Code = model.Code.Trim().ToUpperInvariant();
        model.Zone = string.IsNullOrWhiteSpace(model.Zone) ? "Z1" : model.Zone.Trim().ToUpperInvariant();
        model.Type = string.IsNullOrWhiteSpace(model.Type) ? "Shelf" : model.Type.Trim();
        _db.Locations.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { zone = model.Zone });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var loc = await _db.Locations.FindAsync(id);
        if (loc == null) return NotFound();
        return View(loc);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Location model)
    {
        if (id != model.Id) return NotFound();
        var loc = await _db.Locations.FindAsync(id);
        if (loc == null) return NotFound();
        loc.Zone = model.Zone ?? "Z1";
        loc.X = model.X;
        loc.Y = model.Y;
        loc.Z = model.Z;
        loc.Type = model.Type ?? "Shelf";
        loc.CapacityUnits = model.CapacityUnits;
        loc.IsBlocked = model.IsBlocked;
        await _db.SaveChangesAsync();
        TempData["Msg"] = "Location updated.";
        return RedirectToAction(nameof(Details), new { id = loc.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var loc = await _db.Locations.FindAsync(id);
        if (loc != null)
        {
            _db.Locations.Remove(loc);
            await _db.SaveChangesAsync();
            TempData["Msg"] = "Location deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    public IActionResult BulkGenerate() => View(new BulkGenerateVm { Zone = "Z1", Type = "Shelf", XFrom = 1, XTo = 10, YFrom = 1, YTo = 10, ZFrom = 0, ZTo = 0, CapacityUnits = 100 });

    public class BulkGenerateVm
    {
        public string Zone { get; set; } = "Z1";
        public string Type { get; set; } = "Shelf";
        public int XFrom { get; set; }
        public int XTo { get; set; }
        public int YFrom { get; set; }
        public int YTo { get; set; }
        public int ZFrom { get; set; }
        public int ZTo { get; set; }
        public int CapacityUnits { get; set; } = 100;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkGenerate(BulkGenerateVm vm)
    {
        vm.Zone = (vm.Zone ?? "Z1").Trim().ToUpperInvariant();
        if (vm.XFrom <= 0 || vm.XTo < vm.XFrom || vm.YFrom <= 0 || vm.YTo < vm.YFrom)
        {
            ModelState.AddModelError("", "Invalid ranges.");
            return View(vm);
        }
        var includeZ = vm.ZFrom > 0 && vm.ZTo >= vm.ZFrom;
        var created = 0;
        var skipped = 0;
        for (int x = vm.XFrom; x <= vm.XTo; x++)
        {
            for (int y = vm.YFrom; y <= vm.YTo; y++)
            {
                if (!includeZ)
                {
                    var code = BuildCode(vm.Zone, x, y, null);
                    if (await _db.Locations.AnyAsync(l => l.Code == code)) { skipped++; continue; }
                    _db.Locations.Add(new Location { Code = code, Zone = vm.Zone, X = x, Y = y, Z = null, Type = vm.Type, CapacityUnits = vm.CapacityUnits, IsBlocked = false });
                    created++;
                }
                else
                {
                    for (int z = vm.ZFrom; z <= vm.ZTo; z++)
                    {
                        var code = BuildCode(vm.Zone, x, y, z);
                        if (await _db.Locations.AnyAsync(l => l.Code == code)) { skipped++; continue; }
                        _db.Locations.Add(new Location { Code = code, Zone = vm.Zone, X = x, Y = y, Z = z, Type = vm.Type, CapacityUnits = vm.CapacityUnits, IsBlocked = false });
                        created++;
                    }
                }
            }
        }
        await _db.SaveChangesAsync();
        TempData["Msg"] = $"Created: {created}. Skipped: {skipped}.";
        return RedirectToAction(nameof(Index), new { zone = vm.Zone });
    }

    private static string BuildCode(string zone, int x, int y, int? z)
    {
        var code = $"{zone}-X{x:D2}-Y{y:D2}";
        if (z.HasValue) code += $"-Z{z:D2}";
        return code;
    }
}
