using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManufacturingTimeTracking.Data;
using ManufacturingTimeTracking.Models.Execution;
using ManufacturingTimeTracking.Models.Catalog;

namespace ManufacturingTimeTracking.Pages.Orders;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public BuildOrder BuildOrder { get; set; } = default!;

    public SelectList MachineModels { get; set; } = default!;
    public SelectList MachineVariants { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        MachineModels = new SelectList(await _context.MachineModels.Where(m => m.Active).ToListAsync(), "Id", "Name");
        MachineVariants = new SelectList(new List<MachineVariant>(), "Id", "Name");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || _context.BuildOrders == null || BuildOrder == null)
        {
            MachineModels = new SelectList(await _context.MachineModels.Where(m => m.Active).ToListAsync(), "Id", "Name");
            MachineVariants = new SelectList(new List<MachineVariant>(), "Id", "Name");
            return Page();
        }

        BuildOrder.Status = "Pending";
        BuildOrder.CreatedAt = DateTime.UtcNow;
        _context.BuildOrders.Add(BuildOrder);
        await _context.SaveChangesAsync();

        // Automatically generate pick list from machine model components (including alternatives)
        var machineModel = await _context.MachineModels
            .Include(m => m.Components)
                .ThenInclude(c => c.Alternatives)
            .FirstOrDefaultAsync(m => m.Id == BuildOrder.MachineModelId);

        if (machineModel != null && machineModel.Components.Any())
        {
            foreach (var component in machineModel.Components)
            {
                // Add primary item
                _context.OrderPickLines.Add(new OrderPickLine
                {
                    BuildOrderId = BuildOrder.Id,
                    ItemId = component.ItemId,
                    QuantityRequired = component.Quantity,
                    QuantityPicked = 0
                });
                
                // Add all alternatives
                foreach (var alt in component.Alternatives)
                {
                    _context.OrderPickLines.Add(new OrderPickLine
                    {
                        BuildOrderId = BuildOrder.Id,
                        ItemId = alt.ItemId,
                        QuantityRequired = component.Quantity,
                        QuantityPicked = 0
                    });
                }
            }
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
