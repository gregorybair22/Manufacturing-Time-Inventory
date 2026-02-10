namespace ManufacturingTimeTracking.Models.Execution;

public class BuildOrder
{
    public int Id { get; set; }
    public string ExternalRef { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public int MachineModelId { get; set; }
    public int MachineVariantId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Cancelled
    
    // Navigation
    public ICollection<BuildExecution> Executions { get; set; } = new List<BuildExecution>();
}
