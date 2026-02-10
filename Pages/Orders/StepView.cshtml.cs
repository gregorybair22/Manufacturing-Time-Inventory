using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ManufacturingTimeTracking.Data;
using ManufacturingTimeTracking.Models.Execution;
using ManufacturingTimeTracking.Models.Templates;

namespace ManufacturingTimeTracking.Pages.Orders;

[Authorize]
public class StepViewModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public StepViewModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public StepExec StepExec { get; set; } = default!;
    public BuildOrder BuildOrder { get; set; } = default!;
    public StepRun? CurrentRun { get; set; }
    public List<StepTemplateMedia>? StepTemplateMedia { get; set; }

    public async Task<IActionResult> OnGetAsync(int? stepExecId)
    {
        if (stepExecId == null)
        {
            return NotFound();
        }

        var step = await _context.StepExecs
            .Include(s => s.PhaseExec)
                .ThenInclude(p => p.BuildExecution)
                    .ThenInclude(e => e.BuildOrder)
            .Include(s => s.Evidence)
                .ThenInclude(e => e.Media)
            .Include(s => s.Runs)
                .ThenInclude(r => r.Workstation)
            .Include(s => s.Tools)
            .Include(s => s.Materials)
            .FirstOrDefaultAsync(s => s.Id == stepExecId);

        if (step == null)
        {
            return NotFound();
        }

        StepExec = step;
        BuildOrder = step.PhaseExec.BuildExecution.BuildOrder;
        CurrentRun = step.Runs.FirstOrDefault(r => r.FinishedAt == null);

        // Get step template media by finding the template and matching step by sort order
        var buildOrder = step.PhaseExec.BuildExecution.BuildOrder;
        var template = await _context.ProcessTemplates
            .Include(pt => pt.Phases)
                .ThenInclude(p => p.Steps)
                    .ThenInclude(s => s.Media)
                        .ThenInclude(sm => sm.Media)
            .FirstOrDefaultAsync(pt => pt.MachineModelId == buildOrder.MachineModelId 
                && pt.MachineVariantId == buildOrder.MachineVariantId);

        if (template != null)
        {
            var phaseTemplate = template.Phases.FirstOrDefault(p => p.SortOrder == step.PhaseExec.SortOrder);
            if (phaseTemplate != null)
            {
                var stepTemplate = phaseTemplate.Steps.FirstOrDefault(s => s.SortOrder == step.SortOrder);
                if (stepTemplate != null)
                {
                    StepTemplateMedia = stepTemplate.Media.ToList();
                }
            }
        }

        return Page();
    }
}
