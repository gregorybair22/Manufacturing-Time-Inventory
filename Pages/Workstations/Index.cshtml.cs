using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ManufacturingTimeTracking.Data;
using ManufacturingTimeTracking.Models.Catalog;

namespace ManufacturingTimeTracking.Pages.Workstations;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Workstation> Workstations { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Workstations = await _context.Workstations
            .OrderBy(w => w.Name)
            .ToListAsync();
    }
}
