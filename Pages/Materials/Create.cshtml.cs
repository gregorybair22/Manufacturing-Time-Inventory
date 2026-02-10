using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ManufacturingTimeTracking.Data;
using ManufacturingTimeTracking.Models.Templates;
using System.Security.Claims;

namespace ManufacturingTimeTracking.Pages.Materials;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public CreateModel(ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment environment)
    {
        _context = context;
        _configuration = configuration;
        _environment = environment;
    }

    [BindProperty]
    public Material Material { get; set; } = default!;

    [BindProperty]
    public IFormFile? ImageFile { get; set; }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Handle image upload
        if (ImageFile != null && ImageFile.Length > 0)
        {
            var extension = Path.GetExtension(ImageFile.FileName).ToLower();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            
            if (allowedExtensions.Contains(extension))
            {
                var basePath = _configuration["MediaStorage:BasePath"] ?? "wwwroot/uploads/";
                var materialsPath = Path.Combine(basePath, "materials");

                if (!Directory.Exists(materialsPath))
                {
                    Directory.CreateDirectory(materialsPath);
                }

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(materialsPath, fileName);
                var relativePath = Path.Combine("materials", fileName).Replace('\\', '/');

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                Material.ImageUrl = relativePath;
            }
        }

        _context.Materials.Add(Material);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
