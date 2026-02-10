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

        return RedirectToPage("./Index");
    }
}
