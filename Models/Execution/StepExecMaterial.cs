namespace ManufacturingTimeTracking.Models.Execution;

public class StepExecMaterial
{
    public int Id { get; set; }
    public int StepExecId { get; set; }
    public string MaterialName { get; set; } = string.Empty; // Snapshot as text
    public decimal? Qty { get; set; }
    public string? Unit { get; set; }
    
    // Navigation
    public StepExec StepExec { get; set; } = null!;
}
