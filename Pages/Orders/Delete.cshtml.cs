using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ManufacturingTimeTracking.Models.Execution;

namespace ManufacturingTimeTracking.Pages.Orders;

public class DeleteModel : PageModel
{
    public BuildOrder? BuildOrder { get; set; }

    public IActionResult OnGet(int? id)
    {
        TempData["ToastrError"] = "Orders cannot be deleted.";
        return RedirectToPage("./Index");
    }

    public IActionResult OnPost(int? id)
    {
        TempData["ToastrError"] = "Orders cannot be deleted.";
        return RedirectToPage("./Index");
    }
}
