using System.ComponentModel.DataAnnotations;
using ManufacturingTimeTracking.Models.Inventory;

namespace ManufacturingTimeTracking.Models.Catalog;

/// <summary>
/// Component (spare part) required to build one machine of this model. Used for pick lists when creating orders.
/// If Alternatives exist, any of those items can be picked instead of the primary ItemId.
/// </summary>
public class MachineModelComponent
{
    public int Id { get; set; }
    public int MachineModelId { get; set; }
    /// <summary>Primary item (required if no alternatives). If alternatives exist, this is the default/preferred item.</summary>
    public int ItemId { get; set; }
    /// <summary>Quantity required per machine (e.g. 2 motors, 1 PC).</summary>
    public int Quantity { get; set; } = 1;
    [MaxLength(128)]
    public string? Notes { get; set; }

    public MachineModel MachineModel { get; set; } = null!;
    public Item Item { get; set; } = null!;
    /// <summary>Alternative items that can be used instead of the primary ItemId. If empty, only ItemId can be used.</summary>
    public ICollection<MachineModelComponentAlternative> Alternatives { get; set; } = new List<MachineModelComponentAlternative>();
}
