using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ManufacturingTimeTracking.Data;

namespace ManufacturingTimeTracking.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public int TotalOrders { get; set; }
    public int ActiveOrders { get; set; }
    public int TotalTemplates { get; set; }
    public int TotalModels { get; set; }

    public async Task OnGetAsync()
    {
        TotalOrders = await _context.BuildOrders.CountAsync();
        ActiveOrders = await _context.BuildOrders.CountAsync(o => o.Status == "InProgress");
        TotalTemplates = await _context.ProcessTemplates.CountAsync();
        TotalModels = await _context.MachineModels.CountAsync(m => m.Active);
    }
}
