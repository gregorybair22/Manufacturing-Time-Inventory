using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ManufacturingTimeTracking.Data;
using ManufacturingTimeTracking.Models.Templates;
using ManufacturingTimeTracking.Models.Catalog;

namespace ManufacturingTimeTracking.Pages.Templates;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<ProcessTemplate> ProcessTemplates { get; set; } = default!;
    public Dictionary<int, string> MachineModelNames { get; set; } = new();
    public Dictionary<int, (string Name, string Code)> MachineVariants { get; set; } = new();

    public async Task OnGetAsync()
    {
        ProcessTemplates = await _context.ProcessTemplates
            .Include(pt => pt.Phases)
                .ThenInclude(p => p.Steps)
            .ToListAsync();

        // Load all machine models for name lookup
        var modelIds = ProcessTemplates.Select(pt => pt.MachineModelId).Distinct().ToList();
        var models = await _context.MachineModels
            .Where(m => modelIds.Contains(m.Id))
            .ToListAsync();
        
        MachineModelNames = models.ToDictionary(m => m.Id, m => m.Name);

        // Load all machine variants for name/code lookup
        var variantIds = ProcessTemplates.Select(pt => pt.MachineVariantId).Distinct().ToList();
        var variants = await _context.MachineVariants
            .Where(v => variantIds.Contains(v.Id))
            .ToListAsync();
        
        MachineVariants = variants.ToDictionary(v => v.Id, v => (v.Name, v.Code ?? string.Empty));
    }
}
