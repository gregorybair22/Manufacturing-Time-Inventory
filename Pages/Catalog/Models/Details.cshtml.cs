using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ManufacturingTimeTracking.Data;
using ManufacturingTimeTracking.Models.Catalog;

namespace ManufacturingTimeTracking.Pages.Catalog.Models;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public MachineModel MachineModel { get; set; } = default!;

    [BindProperty]
    public MachineVariant NewVariant { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var machineModel = await _context.MachineModels
            .Include(m => m.Variants)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (machineModel == null)
        {
            return NotFound();
        }

        MachineModel = machineModel;
        NewVariant.MachineModelId = machineModel.Id;
        return Page();
    }

    public async Task<IActionResult> OnPostAddVariantAsync(int? id)
    {
        // Get the model ID from route or form
        var modelId = id ?? NewVariant.MachineModelId;
        
        if (modelId <= 0)
        {
            ModelState.AddModelError("", "Invalid model ID. Please try again.");
            MachineModel = await _context.MachineModels
                .Include(m => m.Variants)
                .FirstOrDefaultAsync(m => m.Id == modelId) ?? default!;
            return Page();
        }

        // Reload the model for display
        MachineModel = await _context.MachineModels
            .Include(m => m.Variants)
            .FirstOrDefaultAsync(m => m.Id == modelId) ?? default!;

        if (MachineModel == null)
        {
            ModelState.AddModelError("", "Machine model not found.");
            return Page();
        }

        // Ensure MachineModelId is set
        NewVariant.MachineModelId = modelId;

        // Validate required fields
        if (string.IsNullOrWhiteSpace(NewVariant.Name))
        {
            ModelState.AddModelError("NewVariant.Name", "Name is required.");
        }

        if (string.IsNullOrWhiteSpace(NewVariant.Code))
        {
            ModelState.AddModelError("NewVariant.Code", "Code is required.");
        }

        // Remove validation errors for navigation property
        ModelState.Remove("NewVariant.MachineModel");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            NewVariant.Active = true;
            // Don't set the navigation property, just the ID
            _context.MachineVariants.Add(NewVariant);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Variant '{NewVariant.Name}' added successfully!";
            return RedirectToPage("./Details", new { id = modelId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error adding variant: {ex.Message}");
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeleteVariantAsync(int variantId, int modelId)
    {
        var variant = await _context.MachineVariants.FindAsync(variantId);
        if (variant != null)
        {
            _context.MachineVariants.Remove(variant);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Details", new { id = modelId });
    }

    public async Task<IActionResult> OnPostUpdateVariantAsync(int variantId, int modelId, string name, string code)
    {
        var variant = await _context.MachineVariants.FindAsync(variantId);
        if (variant == null)
        {
            TempData["Error"] = "Variant not found.";
            return RedirectToPage("./Details", new { id = modelId });
        }

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(code))
        {
            TempData["Error"] = "Name and Code are required.";
            return RedirectToPage("./Details", new { id = modelId });
        }

        variant.Name = name.Trim();
        variant.Code = code.Trim();
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Variant '{variant.Name}' updated successfully!";
        return RedirectToPage("./Details", new { id = modelId });
    }
}
