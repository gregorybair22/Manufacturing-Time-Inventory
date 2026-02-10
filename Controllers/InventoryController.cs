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

    public IActionResult Reports() => View();

    /// <summary>Report: stock grouped by warehouse (location).</summary>
    public async Task<IActionResult> ReportByWarehouse()
    {
        var data = await _db.StockSnapshots
            .Include(s => s.Item)
            .Include(s => s.Location)
            .Where(s => s.Quantity > 0)
            .ToListAsync();
        var byLocation = data
            .GroupBy(s => s.Location?.Code ?? "?")
            .Select(g => new ReportByWarehouseRow
            {
                LocationCode = g.Key,
                LocationName = g.First().Location?.Code ?? g.Key,
                Zone = g.First().Location?.Zone ?? "",
                TotalQuantity = g.Sum(s => s.Quantity),
                LineCount = g.Count(),
                Items = g.Select(s => new ReportStockLine { Sku = s.Item?.Sku ?? "", Name = s.Item?.Name ?? "", Quantity = s.Quantity }).ToList()
            })
            .OrderBy(r => r.LocationCode)
            .ToList();
        return View(byLocation);
    }

    /// <summary>Report: stock grouped by item.</summary>
    public async Task<IActionResult> ReportByItem()
    {
        var data = await _db.StockSnapshots
            .Include(s => s.Item)
            .Include(s => s.Location)
            .Where(s => s.Quantity > 0)
            .ToListAsync();
        var byItem = data
            .GroupBy(s => s.ItemId)
            .Select(g =>
            {
                var first = g.First();
                return new ReportByItemRow
                {
                    ItemId = first.ItemId,
                    Sku = first.Item?.Sku ?? "",
                    Name = first.Item?.Name ?? "",
                    ModelOrType = first.Item?.ModelOrType ?? "",
                    TotalQuantity = g.Sum(s => s.Quantity),
                    Locations = g.Select(s => new ReportLocationLine { LocationCode = s.Location?.Code ?? "?", Quantity = s.Quantity }).ToList()
                };
            })
            .OrderBy(r => r.Sku)
            .ToList();
        return View(byItem);
    }

    /// <summary>Report: stock grouped by item model/type.</summary>
    public async Task<IActionResult> ReportByModel()
    {
        var data = await _db.StockSnapshots
            .Include(s => s.Item)
            .Include(s => s.Location)
            .Where(s => s.Quantity > 0)
            .ToListAsync();
        var byModel = data
            .GroupBy(s => string.IsNullOrWhiteSpace(s.Item?.ModelOrType) ? "(no type)" : s.Item!.ModelOrType)
            .Select(g => new ReportByModelRow
            {
                ModelOrType = g.Key,
                TotalQuantity = g.Sum(s => s.Quantity),
                ItemCount = g.Select(s => s.ItemId).Distinct().Count(),
                Lines = g.GroupBy(x => x.ItemId).Select(ig =>
                {
                    var first = ig.First();
                    return new ReportByModelItemLine { Sku = first.Item?.Sku ?? "", Name = first.Item?.Name ?? "", TotalQty = ig.Sum(s => s.Quantity) };
                }).OrderBy(l => l.Sku).ToList()
            })
            .OrderBy(r => r.ModelOrType)
            .ToList();
        return View(byModel);
    }
}

public class ReportByWarehouseRow
{
    public string LocationCode { get; set; } = "";
    public string LocationName { get; set; } = "";
    public string Zone { get; set; } = "";
    public int TotalQuantity { get; set; }
    public int LineCount { get; set; }
    public List<ReportStockLine> Items { get; set; } = new();
}

public class ReportStockLine
{
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
}

public class ReportByItemRow
{
    public int ItemId { get; set; }
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public string ModelOrType { get; set; } = "";
    public int TotalQuantity { get; set; }
    public List<ReportLocationLine> Locations { get; set; } = new();
}

public class ReportLocationLine
{
    public string LocationCode { get; set; } = "";
    public int Quantity { get; set; }
}

public class ReportByModelRow
{
    public string ModelOrType { get; set; } = "";
    public int TotalQuantity { get; set; }
    public int ItemCount { get; set; }
    public List<ReportByModelItemLine> Lines { get; set; } = new();
}

public class ReportByModelItemLine
{
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public int TotalQty { get; set; }
}
