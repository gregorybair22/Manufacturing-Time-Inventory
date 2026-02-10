using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ManufacturingTimeTracking.Data;

namespace ManufacturingTimeTracking.Pages.Materials;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Models.Templates.Material> Materials { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Materials = await _context.Materials
            .OrderBy(m => m.Name)
            .ToListAsync();
    }
}
