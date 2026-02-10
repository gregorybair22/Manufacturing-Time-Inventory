using ManufacturingTimeTracking.Models.Inventory;

namespace ManufacturingTimeTracking.Models.Templates;

public class Material
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public string? ImageUrl { get; set; }

    // Navigation
    public ICollection<StepTemplateMaterial> StepTemplates { get; set; } = new List<StepTemplateMaterial>();
    /// <summary>Inventory items linked to this material (same product in warehouse and production).</summary>
    public ICollection<Item> InventoryItems { get; set; } = new List<Item>();
}
