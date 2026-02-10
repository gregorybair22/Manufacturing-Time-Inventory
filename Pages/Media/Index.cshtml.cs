using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ManufacturingTimeTracking.Data;
using MediaModel = ManufacturingTimeTracking.Models.Templates.Media;

namespace ManufacturingTimeTracking.Pages.Media;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<MediaModel> MediaList { get; set; } = default!;

    public async Task OnGetAsync()
    {
        MediaList = await _context.Media
            .OrderByDescending(m => m.UploadedAt)
            .ToListAsync();
    }
}
