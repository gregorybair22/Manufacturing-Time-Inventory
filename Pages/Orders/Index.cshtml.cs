using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ManufacturingTimeTracking.Data;
using ManufacturingTimeTracking.Models.Execution;

namespace ManufacturingTimeTracking.Pages.Orders;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<BuildOrder> BuildOrders { get; set; } = default!;

    public async Task OnGetAsync()
    {
        BuildOrders = await _context.BuildOrders
            .Include(o => o.Executions)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }
}
