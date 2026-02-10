namespace ManufacturingTimeTracking.Models.Catalog;

public class MachineModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Active { get; set; } = true;
    
    // Navigation
    public ICollection<MachineVariant> Variants { get; set; } = new List<MachineVariant>();
}
