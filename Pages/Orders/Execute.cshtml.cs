using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ManufacturingTimeTracking.Data;
using ManufacturingTimeTracking.Models.Execution;
using ManufacturingTimeTracking.Models.Templates;
using MediaModel = ManufacturingTimeTracking.Models.Templates.Media;
using System.Security.Claims;

namespace ManufacturingTimeTracking.Pages.Orders;

[Authorize]
public class ExecuteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ExecuteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public BuildOrder BuildOrder { get; set; } = default!;
    public BuildExecution? CurrentExecution { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.BuildOrders
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        var execution = await _context.BuildExecutions
            .Include(e => e.Phases)
                .ThenInclude(p => p.Steps)
                    .ThenInclude(s => s.Runs)
                        .ThenInclude(r => r.Workstation)
            .Include(e => e.Phases)
                .ThenInclude(p => p.Steps)
                    .ThenInclude(s => s.Evidence)
                        .ThenInclude(e => e.Media)
            .FirstOrDefaultAsync(e => e.BuildOrderId == id && e.Status == "InProgress");

        if (order == null)
        {
            return NotFound();
        }

        BuildOrder = order;
        CurrentExecution = execution;

        return Page();
    }

    public async Task<IActionResult> OnPostStartManufacturingAsync(int orderId)
    {
        var order = await _context.BuildOrders
            .Include(o => o.Executions)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            return NotFound();
        }

        // Check if there's already an active execution
        if (order.Executions.Any(e => e.Status == "InProgress"))
        {
            return RedirectToPage("./Execute", new { id = orderId });
        }

        // Find the template for this model/variant
        var template = await _context.ProcessTemplates
            .Include(pt => pt.Phases)
                .ThenInclude(p => p.Steps)
                    .ThenInclude(s => s.Tools)
                        .ThenInclude(st => st.Tool)
            .Include(pt => pt.Phases)
                .ThenInclude(p => p.Steps)
                    .ThenInclude(s => s.Materials)
                        .ThenInclude(sm => sm.Material)
            .FirstOrDefaultAsync(pt => pt.MachineModelId == order.MachineModelId 
                && pt.MachineVariantId == order.MachineVariantId);

        if (template == null)
        {
            TempData["Error"] = "No template found for this model and variant combination.";
            return RedirectToPage("./Execute", new { id = orderId });
        }

        // Create execution
        var execution = new BuildExecution
        {
            BuildOrderId = orderId,
            StartedAt = DateTime.UtcNow,
            Status = "InProgress"
        };
        _context.BuildExecutions.Add(execution);
        await _context.SaveChangesAsync();

        // Snapshot phases and steps
        foreach (var phaseTemplate in template.Phases.OrderBy(p => p.SortOrder))
        {
            var phaseExec = new PhaseExec
            {
                BuildExecutionId = execution.Id,
                SortOrder = phaseTemplate.SortOrder,
                Name = phaseTemplate.Name
            };
            _context.PhaseExecs.Add(phaseExec);
            await _context.SaveChangesAsync();

            foreach (var stepTemplate in phaseTemplate.Steps.OrderBy(s => s.SortOrder))
            {
                var stepExec = new StepExec
                {
                    PhaseExecId = phaseExec.Id,
                    SortOrder = stepTemplate.SortOrder,
                    Title = stepTemplate.Title ?? string.Empty,
                    Instructions = stepTemplate.Instructions ?? string.Empty,
                    AllowSkip = stepTemplate.AllowSkip,
                    State = "Pending"
                };
                _context.StepExecs.Add(stepExec);
                await _context.SaveChangesAsync(); // Save to get stepExec.Id

                // Copy Tools from template
                foreach (var toolTemplate in stepTemplate.Tools)
                {
                    var stepExecTool = new StepExecTool
                    {
                        StepExecId = stepExec.Id,
                        ToolName = toolTemplate.Tool?.Name ?? "Unknown Tool"
                    };
                    _context.StepExecTools.Add(stepExecTool);
                }

                // Copy Materials from template
                foreach (var materialTemplate in stepTemplate.Materials)
                {
                    var stepExecMaterial = new StepExecMaterial
                    {
                        StepExecId = stepExec.Id,
                        MaterialName = materialTemplate.Material?.Name ?? "Unknown Material",
                        Qty = materialTemplate.Qty,
                        Unit = materialTemplate.Material?.Unit
                    };
                    _context.StepExecMaterials.Add(stepExecMaterial);
                }
                
                // Save tools and materials for this step
                await _context.SaveChangesAsync();
            }
        }

        order.Status = "InProgress";
        await _context.SaveChangesAsync();

        return RedirectToPage("./Execute", new { id = orderId });
    }

    public async Task<IActionResult> OnPostStartStepAsync(int stepExecId)
    {
        var step = await _context.StepExecs
            .Include(s => s.PhaseExec)
                .ThenInclude(p => p.BuildExecution)
            .FirstOrDefaultAsync(s => s.Id == stepExecId);

        if (step == null)
        {
            return NotFound();
        }

        // Check if there's already a running step
        var runningStep = await _context.StepRuns
            .FirstOrDefaultAsync(sr => sr.StepExecId == stepExecId && sr.FinishedAt == null);

        if (runningStep != null)
        {
            return RedirectToPage("./Execute", new { id = step.PhaseExec.BuildExecution.BuildOrderId });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var stepRun = new StepRun
        {
            StepExecId = stepExecId,
            StartedAt = DateTime.UtcNow,
            UserId = userId ?? ""
        };
        _context.StepRuns.Add(stepRun);

        step.State = "InProgress";
        await _context.SaveChangesAsync();

        // Redirect to full-screen step view
        return RedirectToPage("./StepView", new { stepExecId = stepExecId });
    }

    public async Task<IActionResult> OnPostStopStepAsync(int stepExecId)
    {
        var step = await _context.StepExecs
            .Include(s => s.PhaseExec)
                .ThenInclude(p => p.BuildExecution)
            .FirstOrDefaultAsync(s => s.Id == stepExecId);

        if (step == null)
        {
            return NotFound();
        }

        var runningStep = await _context.StepRuns
            .FirstOrDefaultAsync(sr => sr.StepExecId == stepExecId && sr.FinishedAt == null);

        if (runningStep != null)
        {
            runningStep.FinishedAt = DateTime.UtcNow;
            runningStep.DurationSeconds = (int)(runningStep.FinishedAt.Value - runningStep.StartedAt).TotalSeconds;
            _context.StepRuns.Update(runningStep);
        }

        step.State = "Done";
        await _context.SaveChangesAsync();

        return RedirectToPage("./Execute", new { id = step.PhaseExec.BuildExecution.BuildOrderId });
    }

    public async Task<IActionResult> OnPostSkipStepAsync(int stepExecId, string skipReason)
    {
        var step = await _context.StepExecs
            .Include(s => s.PhaseExec)
                .ThenInclude(p => p.BuildExecution)
            .FirstOrDefaultAsync(s => s.Id == stepExecId);

        if (step == null)
        {
            return NotFound();
        }

        if (!step.AllowSkip)
        {
            TempData["Error"] = "This step cannot be skipped.";
            return RedirectToPage("./Execute", new { id = step.PhaseExec.BuildExecution.BuildOrderId });
        }

        step.State = "Skipped";
        step.SkipReason = skipReason;
        await _context.SaveChangesAsync();

        return RedirectToPage("./Execute", new { id = step.PhaseExec.BuildExecution.BuildOrderId });
    }

    public async Task<IActionResult> OnPostStartStepWithWorkstationAsync(int stepExecId, int? workstationId)
    {
        var step = await _context.StepExecs
            .Include(s => s.PhaseExec)
                .ThenInclude(p => p.BuildExecution)
            .FirstOrDefaultAsync(s => s.Id == stepExecId);

        if (step == null)
        {
            return NotFound();
        }

        // Check if there's already a running step
        var runningStep = await _context.StepRuns
            .FirstOrDefaultAsync(sr => sr.StepExecId == stepExecId && sr.FinishedAt == null);

        if (runningStep != null)
        {
            return RedirectToPage("./Execute", new { id = step.PhaseExec.BuildExecution.BuildOrderId });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var stepRun = new StepRun
        {
            StepExecId = stepExecId,
            StartedAt = DateTime.UtcNow,
            UserId = userId ?? "",
            WorkstationId = workstationId
        };
        _context.StepRuns.Add(stepRun);

        step.State = "InProgress";
        await _context.SaveChangesAsync();

        // Redirect to full-screen step view
        return RedirectToPage("./StepView", new { stepExecId = stepExecId });
    }

    public async Task<IActionResult> OnPostUploadEvidenceAsync(int stepExecId, IFormFile? imageFile, string? note)
    {
        if (!User.IsInRole("Admin") && !User.IsInRole("Supervisor"))
        {
            return Forbid();
        }

        var step = await _context.StepExecs
            .Include(s => s.PhaseExec)
                .ThenInclude(p => p.BuildExecution)
            .FirstOrDefaultAsync(s => s.Id == stepExecId);

        if (step == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var basePath = configuration["MediaStorage:BasePath"] ?? "wwwroot/uploads/";
        var evidencePath = configuration["MediaStorage:EvidencePath"] ?? "evidence";
        var uploadPath = Path.Combine(basePath, evidencePath, step.PhaseExec.BuildExecution.BuildOrderId.ToString(), stepExecId.ToString());

        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        if (imageFile != null && imageFile.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var filePath = Path.Combine(uploadPath, fileName);
            var relativePath = Path.Combine(evidencePath, step.PhaseExec.BuildExecution.BuildOrderId.ToString(), stepExecId.ToString(), fileName).Replace('\\', '/');

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Determine media type based on file extension
            var extension = Path.GetExtension(imageFile.FileName).ToLower();
            var mediaType = (extension == ".mp4" || extension == ".avi" || extension == ".mov" || extension == ".wmv" || extension == ".flv" || extension == ".webm") 
                ? "Video" 
                : "Image";

            var media = new MediaModel
            {
                Type = mediaType,
                UrlOrPath = relativePath,
                UploadedBy = userId ?? "",
                UploadedAt = DateTime.UtcNow
            };
            _context.Media.Add(media);
            await _context.SaveChangesAsync();

            // Create instruction (reference material for future users)
            var evidence = new StepEvidence
            {
                StepExecId = stepExecId,
                MediaId = media.Id,
                Note = note,
                UserId = userId ?? "",
                CreatedAt = DateTime.UtcNow
            };
            _context.StepEvidence.Add(evidence);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Instruction added successfully! This will help future users understand how to perform this step.";
        }
        else if (!string.IsNullOrWhiteSpace(note))
        {
            // Create a placeholder media entry for notes-only instructions
            var media = new MediaModel
            {
                Type = "Note",
                UrlOrPath = "",
                UploadedBy = userId ?? "",
                UploadedAt = DateTime.UtcNow
            };
            _context.Media.Add(media);
            await _context.SaveChangesAsync();

            // Create instruction (reference material for future users)
            var evidence = new StepEvidence
            {
                StepExecId = stepExecId,
                MediaId = media.Id,
                Note = note,
                UserId = userId ?? "",
                CreatedAt = DateTime.UtcNow
            };
            _context.StepEvidence.Add(evidence);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Instruction added successfully! This will help future users understand how to perform this step.";
        }
        else
        {
            TempData["Error"] = "Please provide either an image/video or written instructions.";
        }

        return RedirectToPage("./Execute", new { id = step.PhaseExec.BuildExecution.BuildOrderId });
    }

    public async Task<IActionResult> OnPostAddStepAsync(int phaseExecId, string title, string instructions, bool allowSkip)
    {
        if (!User.IsInRole("Admin") && !User.IsInRole("Supervisor"))
        {
            return Forbid();
        }

        var phase = await _context.PhaseExecs
            .Include(p => p.BuildExecution)
            .FirstOrDefaultAsync(p => p.Id == phaseExecId);

        if (phase == null)
        {
            return NotFound();
        }

        // Get the highest sort order in this phase
        var maxSortOrder = (await _context.StepExecs
            .Where(s => s.PhaseExecId == phaseExecId)
            .Select(s => (int?)s.SortOrder)
            .MaxAsync()) ?? 0;

        var newStep = new StepExec
        {
            PhaseExecId = phaseExecId,
            SortOrder = maxSortOrder + 1,
            Title = title ?? string.Empty,
            Instructions = instructions ?? string.Empty,
            AllowSkip = allowSkip,
            State = "Pending"
        };

        _context.StepExecs.Add(newStep);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Execute", new { id = phase.BuildExecution.BuildOrderId });
    }
}
