using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ManufacturingTimeTracking.Models;
using ManufacturingTimeTracking.Models.Catalog;
using ManufacturingTimeTracking.Models.Templates;
using ManufacturingTimeTracking.Models.Execution;
using ManufacturingTimeTracking.Models.Inventory;

namespace ManufacturingTimeTracking.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Catalog
    public DbSet<MachineModel> MachineModels { get; set; }
    public DbSet<MachineVariant> MachineVariants { get; set; }
    public DbSet<MachineModelComponent> MachineModelComponents { get; set; }
    public DbSet<Workstation> Workstations { get; set; }

    // Templates
    public DbSet<ProcessTemplate> ProcessTemplates { get; set; }
    public DbSet<PhaseTemplate> PhaseTemplates { get; set; }
    public DbSet<StepTemplate> StepTemplates { get; set; }
    public DbSet<Tool> Tools { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<Media> Media { get; set; }
    public DbSet<StepTemplateTool> StepTemplateTools { get; set; }
    public DbSet<StepTemplateMaterial> StepTemplateMaterials { get; set; }
    public DbSet<StepTemplateMedia> StepTemplateMedia { get; set; }

    // Execution
    public DbSet<BuildOrder> BuildOrders { get; set; }
    public DbSet<BuildExecution> BuildExecutions { get; set; }
    public DbSet<PhaseExec> PhaseExecs { get; set; }
    public DbSet<StepExec> StepExecs { get; set; }
    public DbSet<StepExecTool> StepExecTools { get; set; }
    public DbSet<StepExecMaterial> StepExecMaterials { get; set; }
    public DbSet<StepRun> StepRuns { get; set; }
    public DbSet<StepEvidence> StepEvidence { get; set; }

    // Inventory (shared database)
    public DbSet<Item> Items { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<StockSnapshot> StockSnapshots { get; set; }
    public DbSet<Movement> Movements { get; set; }
    public DbSet<DeviceEvent> DeviceEvents { get; set; }
    public DbSet<RobotTask> RobotTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure relationships
        builder.Entity<MachineVariant>()
            .HasOne(v => v.MachineModel)
            .WithMany(m => m.Variants)
            .HasForeignKey(v => v.MachineModelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MachineModelComponent>()
            .HasOne(c => c.MachineModel)
            .WithMany(m => m.Components)
            .HasForeignKey(c => c.MachineModelId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<MachineModelComponent>()
            .HasOne(c => c.Item)
            .WithMany()
            .HasForeignKey(c => c.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PhaseTemplate>()
            .HasOne(p => p.ProcessTemplate)
            .WithMany(pt => pt.Phases)
            .HasForeignKey(p => p.ProcessTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<StepTemplate>()
            .HasOne(s => s.PhaseTemplate)
            .WithMany(p => p.Steps)
            .HasForeignKey(s => s.PhaseTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<StepTemplateTool>()
            .HasOne(st => st.StepTemplate)
            .WithMany(s => s.Tools)
            .HasForeignKey(st => st.StepTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<StepTemplateTool>()
            .HasOne(st => st.Tool)
            .WithMany(t => t.StepTemplates)
            .HasForeignKey(st => st.ToolId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StepTemplateMaterial>()
            .HasOne(sm => sm.StepTemplate)
            .WithMany(s => s.Materials)
            .HasForeignKey(sm => sm.StepTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<StepTemplateMaterial>()
            .HasOne(sm => sm.Material)
            .WithMany(m => m.StepTemplates)
            .HasForeignKey(sm => sm.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<StepTemplateMaterial>()
            .Property(sm => sm.Qty)
            .HasPrecision(18, 4);

        builder.Entity<StepTemplateMedia>()
            .HasOne(sm => sm.StepTemplate)
            .WithMany(s => s.Media)
            .HasForeignKey(sm => sm.StepTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<StepTemplateMedia>()
            .HasOne(sm => sm.Media)
            .WithMany(m => m.StepTemplates)
            .HasForeignKey(sm => sm.MediaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<BuildExecution>()
            .HasOne(be => be.BuildOrder)
            .WithMany(bo => bo.Executions)
            .HasForeignKey(be => be.BuildOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PhaseExec>()
            .HasOne(p => p.BuildExecution)
            .WithMany(be => be.Phases)
            .HasForeignKey(p => p.BuildExecutionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<StepExec>()
            .HasOne(s => s.PhaseExec)
            .WithMany(p => p.Steps)
            .HasForeignKey(s => s.PhaseExecId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<StepExecTool>()
            .HasOne(st => st.StepExec)
            .WithMany(s => s.Tools)
            .HasForeignKey(st => st.StepExecId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<StepExecMaterial>()
            .HasOne(sm => sm.StepExec)
            .WithMany(s => s.Materials)
            .HasForeignKey(sm => sm.StepExecId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<StepExecMaterial>()
            .Property(sm => sm.Qty)
            .HasPrecision(18, 4);

        builder.Entity<StepRun>()
            .HasOne(sr => sr.StepExec)
            .WithMany(s => s.Runs)
            .HasForeignKey(sr => sr.StepExecId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<StepEvidence>()
            .HasOne(se => se.StepExec)
            .WithMany(s => s.Evidence)
            .HasForeignKey(se => se.StepExecId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<StepEvidence>()
            .HasOne(se => se.Media)
            .WithMany(m => m.StepEvidence)
            .HasForeignKey(se => se.MediaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Inventory
        builder.Entity<Tag>()
            .HasIndex(x => x.Code)
            .IsUnique();
        builder.Entity<Tag>()
            .HasOne(t => t.Item)
            .WithMany(i => i.Tags)
            .HasForeignKey(t => t.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Location>()
            .HasIndex(x => x.Code)
            .IsUnique();

        builder.Entity<StockSnapshot>()
            .HasIndex(x => new { x.ItemId, x.LocationId })
            .IsUnique();
        builder.Entity<StockSnapshot>()
            .HasOne(s => s.Item)
            .WithMany()
            .HasForeignKey(s => s.ItemId);
        builder.Entity<StockSnapshot>()
            .HasOne(s => s.Location)
            .WithMany()
            .HasForeignKey(s => s.LocationId);

        builder.Entity<Movement>()
            .HasOne(m => m.Item)
            .WithMany()
            .HasForeignKey(m => m.ItemId);
        builder.Entity<Movement>()
            .HasOne(m => m.FromLocation)
            .WithMany()
            .HasForeignKey(m => m.FromLocationId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Movement>()
            .HasOne(m => m.ToLocation)
            .WithMany()
            .HasForeignKey(m => m.ToLocationId)
            .OnDelete(DeleteBehavior.NoAction);

        // Item <-> Material link (shared items between inventory and production)
        builder.Entity<Item>()
            .HasOne(i => i.Material)
            .WithMany(m => m.InventoryItems)
            .HasForeignKey(i => i.MaterialId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<RobotTask>()
            .HasOne(t => t.Item)
            .WithMany()
            .HasForeignKey(t => t.ItemId);
        builder.Entity<RobotTask>()
            .HasOne(t => t.FromLocation)
            .WithMany()
            .HasForeignKey(t => t.FromLocationId);
        builder.Entity<RobotTask>()
            .HasOne(t => t.ToLocation)
            .WithMany()
            .HasForeignKey(t => t.ToLocationId);
    }
}
