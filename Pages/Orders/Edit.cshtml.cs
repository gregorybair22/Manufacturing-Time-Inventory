using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManufacturingTimeTracking.Data;
using ManufacturingTimeTracking.Models.Execution;
using ManufacturingTimeTracking.Models.Catalog;

namespace ManufacturingTimeTracking.Pages.Orders;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public BuildOrder BuildOrder { get; set; } = default!;

    public SelectList MachineModels { get; set; } = default!;
    public SelectList MachineVariants { get; set; } = default!;

    public IActionResult OnGet(int? id)
    {
        TempData["ToastrError"] = "Orders cannot be edited.";
        return RedirectToPage("./Index");
    }

    public IActionResult OnPost(int? id)
    {
        TempData["ToastrError"] = "Orders cannot be edited.";
        return RedirectToPage("./Index");
    }
}
