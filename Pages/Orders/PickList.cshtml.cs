using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ManufacturingTimeTracking.Data;
using ManufacturingTimeTracking.Models.Execution;
using ManufacturingTimeTracking.Models.Catalog;

namespace ManufacturingTimeTracking.Pages.Orders;

public class PickListModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public PickListModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public BuildOrder Order { get; set; } = null!;
    public string MachineModelName { get; set; } = "";
    public List<PickListLine> Lines { get; set; } = new();
    public int OrderId { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();

        var order = await _context.BuildOrders.FindAsync(id);
        if (order == null) return NotFound();
        Order = order;
        OrderId = order.Id;

        var model = await _context.MachineModels
            .Include(m => m.Components)
                .ThenInclude(c => c.Item)
            .Include(m => m.Components)
                .ThenInclude(c => c.Alternatives)
                    .ThenInclude(a => a.Item)
            .FirstOrDefaultAsync(m => m.Id == order.MachineModelId);
        if (model == null)
        {
            MachineModelName = $"Model {order.MachineModelId}";
            await LoadLinesFromPickLinesAsync(order.Id);
            return Page();
        }

        MachineModelName = model.Name;

        // Ensure OrderPickLine rows exist (persistent list for this order)
        // Include primary item and all alternatives
        var existing = await _context.OrderPickLines
            .Where(l => l.BuildOrderId == order.Id)
            .Select(l => l.ItemId)
            .ToListAsync();
        
        foreach (var comp in model.Components)
        {
            // Add primary item if not exists
            if (!existing.Contains(comp.ItemId))
            {
                _context.OrderPickLines.Add(new OrderPickLine
                {
                    BuildOrderId = order.Id,
                    ItemId = comp.ItemId,
                    QuantityRequired = comp.Quantity,
                    QuantityPicked = 0
                });
            }
            
            // Add all alternatives if not exists
            foreach (var alt in comp.Alternatives)
            {
                if (!existing.Contains(alt.ItemId))
                {
                    _context.OrderPickLines.Add(new OrderPickLine
                    {
                        BuildOrderId = order.Id,
                        ItemId = alt.ItemId,
                        QuantityRequired = comp.Quantity,
                        QuantityPicked = 0
                    });
                }
            }
        }
        await _context.SaveChangesAsync();

        await LoadLinesFromPickLinesAsync(order.Id);
        return Page();
    }

    private async Task LoadLinesFromPickLinesAsync(int buildOrderId)
    {
        // Get order and model to understand component groups
        var order = await _context.BuildOrders.FindAsync(buildOrderId);
        if (order == null) return;

        var model = await _context.MachineModels
            .Include(m => m.Components)
                .ThenInclude(c => c.Item)
            .Include(m => m.Components)
                .ThenInclude(c => c.Alternatives)
                    .ThenInclude(a => a.Item)
            .FirstOrDefaultAsync(m => m.Id == order.MachineModelId);

        var pickLines = await _context.OrderPickLines
            .Include(l => l.Item)
            .Where(l => l.BuildOrderId == buildOrderId)
            .ToListAsync();

        var itemIds = pickLines.Select(l => l.ItemId).Distinct().ToList();
        var stockByItem = await _context.StockSnapshots
            .Include(s => s.Location)
            .Where(s => itemIds.Contains(s.ItemId) && s.Quantity > 0)
            .ToListAsync();

        var lines = new List<PickListLine>();
        
        if (model != null)
        {
            // Group by component requirement (primary + alternatives)
            foreach (var comp in model.Components)
            {
                var allItemIds = new List<int> { comp.ItemId };
                allItemIds.AddRange(comp.Alternatives.Select(a => a.ItemId));
                
                var componentPickLines = pickLines.Where(pl => allItemIds.Contains(pl.ItemId)).ToList();
                var totalPicked = componentPickLines.Sum(pl => pl.QuantityPicked);
                var quantityNeeded = comp.Quantity;
                
                // Create line for primary item (shows alternatives)
                var primaryPl = componentPickLines.FirstOrDefault(pl => pl.ItemId == comp.ItemId) 
                    ?? new OrderPickLine { ItemId = comp.ItemId, QuantityRequired = quantityNeeded, QuantityPicked = 0 };
                
                var primaryStock = stockByItem.Where(s => s.ItemId == comp.ItemId).OrderBy(s => s.Location?.Zone).ThenBy(s => s.Location?.Code).ToList();
                
                var alternativeItems = new List<PickListAlternative>();
                foreach (var alt in comp.Alternatives.OrderBy(a => a.SortOrder).ThenBy(a => a.Item.Sku))
                {
                    var altPl = componentPickLines.FirstOrDefault(pl => pl.ItemId == alt.ItemId);
                    var altStock = stockByItem.Where(s => s.ItemId == alt.ItemId).OrderBy(s => s.Location?.Zone).ThenBy(s => s.Location?.Code).ToList();
                    alternativeItems.Add(new PickListAlternative
                    {
                        ItemId = alt.ItemId,
                        Sku = alt.Item.Sku,
                        Name = alt.Item.Name,
                        ModelOrType = alt.Item.ModelOrType ?? "",
                        QuantityPicked = altPl?.QuantityPicked ?? 0,
                        TotalAvailable = altStock.Sum(s => s.Quantity),
                        Locations = altStock.Select(s => new PickListLocation { Code = s.Location?.Code ?? "?", Zone = s.Location?.Zone ?? "", Quantity = s.Quantity }).ToList()
                    });
                }
                
                lines.Add(new PickListLine
                {
                    ComponentId = comp.Id,
                    ItemId = comp.ItemId,
                    Sku = comp.Item.Sku,
                    Name = comp.Item.Name,
                    ModelOrType = comp.Item.ModelOrType ?? "",
                    QuantityNeeded = quantityNeeded,
                    QuantityPicked = totalPicked, // Sum of all alternatives picked
                    Notes = comp.Notes,
                    Locations = primaryStock.Select(s => new PickListLocation { Code = s.Location?.Code ?? "?", Zone = s.Location?.Zone ?? "", Quantity = s.Quantity }).ToList(),
                    TotalAvailable = primaryStock.Sum(s => s.Quantity) + alternativeItems.Sum(a => a.TotalAvailable),
                    Alternatives = alternativeItems
                });
            }
        }
        else
        {
            // Fallback: show all pick lines as-is (no grouping)
            foreach (var pl in pickLines)
            {
                var stock = stockByItem.Where(s => s.ItemId == pl.ItemId).OrderBy(s => s.Location?.Zone).ThenBy(s => s.Location?.Code).ToList();
                lines.Add(new PickListLine
                {
                    ItemId = pl.ItemId,
                    Sku = pl.Item?.Sku ?? "",
                    Name = pl.Item?.Name ?? "",
                    ModelOrType = pl.Item?.ModelOrType ?? "",
                    QuantityNeeded = pl.QuantityRequired,
                    QuantityPicked = pl.QuantityPicked,
                    Notes = null,
                    Locations = stock.Select(s => new PickListLocation { Code = s.Location?.Code ?? "?", Zone = s.Location?.Zone ?? "", Quantity = s.Quantity }).ToList(),
                    TotalAvailable = stock.Sum(s => s.Quantity)
                });
            }
        }

        // Sort by warehouse position so person can walk shelves: by first location zone then code
        Lines = lines
            .OrderBy(l => l.Locations.FirstOrDefault()?.Zone ?? l.Alternatives.FirstOrDefault()?.Locations.FirstOrDefault()?.Zone ?? "zzz")
            .ThenBy(l => l.Locations.FirstOrDefault()?.Code ?? l.Alternatives.FirstOrDefault()?.Locations.FirstOrDefault()?.Code ?? "zzz")
            .ThenBy(l => l.Sku)
            .ToList();
    }

    /// <summary>Scan QR (tag code or SKU) to mark one as picked.</summary>
    public async Task<IActionResult> OnPostScanAsync(int? id, string? scannedCode)
    {
        if (id == null)
        {
            TempData["PickListError"] = "Order not specified.";
            return RedirectToPage("./Index");
        }
        if (string.IsNullOrWhiteSpace(scannedCode))
        {
            TempData["PickListError"] = "Enter or scan the QR code (tag code or SKU).";
            return RedirectToPage("./PickList", new { id });
        }

        var code = scannedCode.Trim();

        // Resolve to ItemId: by Tag code (what's on the QR label) or by SKU
        var itemId = await _context.Tags
            .Where(t => t.Code == code)
            .Select(t => (int?)t.ItemId)
            .FirstOrDefaultAsync();
        if (itemId == null)
            itemId = await _context.Items.Where(i => i.Sku == code).Select(i => (int?)i.Id).FirstOrDefaultAsync();

        if (itemId == null)
        {
            TempData["PickListError"] = $"Code '{code}' not found. Scan the item's QR label or enter a valid tag code / SKU.";
            return RedirectToPage("./PickList", new { id });
        }

        // Find pick line - check if this item is primary or an alternative
        var pickLine = await _context.OrderPickLines
            .FirstOrDefaultAsync(l => l.BuildOrderId == id && l.ItemId == itemId.Value);
        
        if (pickLine == null)
        {
            TempData["PickListError"] = "This item is not on the pick list for this order.";
            return RedirectToPage("./PickList", new { id });
        }

        // Check if this is part of a component with alternatives
        var order = await _context.BuildOrders.FindAsync(id);
        if (order != null)
        {
            var model = await _context.MachineModels
                .Include(m => m.Components)
                    .ThenInclude(c => c.Alternatives)
                .FirstOrDefaultAsync(m => m.Id == order.MachineModelId);
            
            if (model != null)
            {
                var component = model.Components.FirstOrDefault(c => 
                    c.ItemId == itemId.Value || c.Alternatives.Any(a => a.ItemId == itemId.Value));
                
                if (component != null)
                {
                    // Get all pick lines for this component (primary + alternatives)
                    var allItemIds = new List<int> { component.ItemId };
                    allItemIds.AddRange(component.Alternatives.Select(a => a.ItemId));
                    var componentPickLines = await _context.OrderPickLines
                        .Where(l => l.BuildOrderId == id && allItemIds.Contains(l.ItemId))
                        .ToListAsync();
                    
                    var totalPicked = componentPickLines.Sum(pl => pl.QuantityPicked);
                    if (totalPicked >= component.Quantity)
                    {
                        var it = await _context.Items.FindAsync(itemId.Value);
                        TempData["PickListSuccess"] = $"Component requirement already satisfied ({totalPicked}/{component.Quantity}). Any alternative can be used.";
                        return RedirectToPage("./PickList", new { id });
                    }
                }
            }
        }

        if (pickLine.QuantityPicked >= pickLine.QuantityRequired)
        {
            var it = await _context.Items.FindAsync(itemId.Value);
            TempData["PickListSuccess"] = $"{it?.Sku ?? "Item"} already fully picked ({pickLine.QuantityPicked}/{pickLine.QuantityRequired}).";
            return RedirectToPage("./PickList", new { id });
        }

        pickLine.QuantityPicked += 1;
        await _context.SaveChangesAsync();

        var item = await _context.Items.FindAsync(itemId.Value);
        var sku = item?.Sku ?? "Item";
        TempData["PickListSuccess"] = $"Picked {sku}. ({pickLine.QuantityPicked}/{pickLine.QuantityRequired})";
        return RedirectToPage("./PickList", new { id });
    }
}

public class PickListLine
{
    public int? ComponentId { get; set; }
    public int ItemId { get; set; }
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public string ModelOrType { get; set; } = "";
    public int QuantityNeeded { get; set; }
    public int QuantityPicked { get; set; }
    public string? Notes { get; set; }
    public int TotalAvailable { get; set; }
    public List<PickListLocation> Locations { get; set; } = new();
    public List<PickListAlternative> Alternatives { get; set; } = new();
}

public class PickListAlternative
{
    public int ItemId { get; set; }
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public string ModelOrType { get; set; } = "";
    public int QuantityPicked { get; set; }
    public int TotalAvailable { get; set; }
    public List<PickListLocation> Locations { get; set; } = new();
}

public class PickListLocation
{
    public string Code { get; set; } = "";
    public string Zone { get; set; } = "";
    public int Quantity { get; set; }
}
