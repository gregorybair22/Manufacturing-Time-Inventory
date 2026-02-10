namespace ManufacturingTimeTracking.Models.Execution;

public class StepExec
{
    public int Id { get; set; }
    public int PhaseExecId { get; set; }
    public int SortOrder { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public bool AllowSkip { get; set; } = true;
    public string State { get; set; } = "Pending"; // Pending, InProgress, Done, Skipped
    public string? SkipReason { get; set; }
    
    // Navigation
    public PhaseExec PhaseExec { get; set; } = null!;
    public ICollection<StepExecTool> Tools { get; set; } = new List<StepExecTool>();
    public ICollection<StepExecMaterial> Materials { get; set; } = new List<StepExecMaterial>();
    public ICollection<StepRun> Runs { get; set; } = new List<StepRun>();
    public ICollection<StepEvidence> Evidence { get; set; } = new List<StepEvidence>();
}
