using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ManufacturingTimeTracking.Data;
using ManufacturingTimeTracking.Models.Catalog;

namespace ManufacturingTimeTracking.Pages.Catalog.Models;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<MachineModel> MachineModels { get; set; } = default!;

    public async Task OnGetAsync()
    {
        MachineModels = await _context.MachineModels
            .Include(m => m.Variants)
            .Where(m => m.Active)
            .OrderBy(m => m.Name)
            .ToListAsync();
    }
}
