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

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();

        var order = await _context.BuildOrders.FindAsync(id);
        if (order == null) return NotFound();
        Order = order;

        var model = await _context.MachineModels
            .Include(m => m.Components)
                .ThenInclude(c => c.Item)
            .FirstOrDefaultAsync(m => m.Id == order.MachineModelId);
        if (model == null)
        {
            MachineModelName = $"Model {order.MachineModelId}";
            return Page();
        }

        MachineModelName = model.Name;

        foreach (var comp in model.Components.OrderBy(c => c.Item?.Sku))
        {
            var stock = await _context.StockSnapshots
                .Include(s => s.Location)
                .Where(s => s.ItemId == comp.ItemId && s.Quantity > 0)
                .OrderByDescending(s => s.Quantity)
                .ToListAsync();

            Lines.Add(new PickListLine
            {
                Sku = comp.Item?.Sku ?? "",
                Name = comp.Item?.Name ?? "",
                ModelOrType = comp.Item?.ModelOrType ?? "",
                QuantityNeeded = comp.Quantity,
                Notes = comp.Notes,
                Locations = stock.Select(s => new PickListLocation { Code = s.Location?.Code ?? "?", Zone = s.Location?.Zone ?? "", Quantity = s.Quantity }).ToList(),
                TotalAvailable = stock.Sum(s => s.Quantity)
            });
        }

        return Page();
    }
}

public class PickListLine
{
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public string ModelOrType { get; set; } = "";
    public int QuantityNeeded { get; set; }
    public string? Notes { get; set; }
    public int TotalAvailable { get; set; }
    public List<PickListLocation> Locations { get; set; } = new();
}

public class PickListLocation
{
    public string Code { get; set; } = "";
    public string Zone { get; set; } = "";
    public int Quantity { get; set; }
}
