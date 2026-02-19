using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ManufacturingTimeTracking.Data;
using ManufacturingTimeTracking.Models.Execution;
using ManufacturingTimeTracking.Models.Catalog;

namespace ManufacturingTimeTracking.Pages.Orders;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<BuildOrder> BuildOrders { get; set; } = default!;
    public Dictionary<int, string> ModelNames { get; set; } = new();
    public Dictionary<int, string> VariantNames { get; set; } = new();

    public async Task OnGetAsync()
    {
        BuildOrders = await _context.BuildOrders
            .Include(o => o.Executions)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        var modelIds = BuildOrders.Select(o => o.MachineModelId).Distinct().ToList();
        var variantIds = BuildOrders.Select(o => o.MachineVariantId).Distinct().ToList();

        if (modelIds.Any())
        {
            var models = await _context.MachineModels.Where(m => modelIds.Contains(m.Id)).ToListAsync();
            ModelNames = models.ToDictionary(m => m.Id, m => m.Name);
        }
        if (variantIds.Any())
        {
            var variants = await _context.MachineVariants.Where(v => variantIds.Contains(v.Id)).ToListAsync();
            VariantNames = variants.ToDictionary(v => v.Id, v => v.Name);
        }
    }
}
