using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ManufacturingTimeTracking.Data;

namespace ManufacturingTimeTracking.Pages.Tools;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Models.Templates.Tool> Tools { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Tools = await _context.Tools
            .OrderBy(t => t.Name)
            .ToListAsync();
    }
}
