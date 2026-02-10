using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ManufacturingTimeTracking.Data;
using ManufacturingTimeTracking.Models.Execution;

namespace ManufacturingTimeTracking.Pages.Orders;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public BuildOrder BuildOrder { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.BuildOrders
            .Include(o => o.Executions)
                .ThenInclude(e => e.Phases)
                    .ThenInclude(p => p.Steps)
                        .ThenInclude(s => s.Runs)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        BuildOrder = order;
        return Page();
    }
}
