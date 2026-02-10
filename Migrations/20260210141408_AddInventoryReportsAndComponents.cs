using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManufacturingTimeTracking.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryReportsAndComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Qty",
                table: "StepTemplateMaterials",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Qty",
                table: "StepExecMaterials",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            // Add ModelOrType only if missing (may already exist from Program.cs/DatabaseStartup)
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Items]') AND name = 'ModelOrType')
ALTER TABLE [Items] ADD [ModelOrType] nvarchar(64) NOT NULL DEFAULT N'';");

            // Create MachineModelComponents only if missing
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[MachineModelComponents]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[MachineModelComponents] (
        [Id] int NOT NULL IDENTITY,
        [MachineModelId] int NOT NULL,
        [ItemId] int NOT NULL,
        [Quantity] int NOT NULL,
        [Notes] nvarchar(128) NULL,
        CONSTRAINT [PK_MachineModelComponents] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MachineModelComponents_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MachineModelComponents_MachineModels_MachineModelId] FOREIGN KEY ([MachineModelId]) REFERENCES [dbo].[MachineModels] ([Id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_MachineModelComponents_ItemId] ON [dbo].[MachineModelComponents] ([ItemId]);
    CREATE INDEX [IX_MachineModelComponents_MachineModelId] ON [dbo].[MachineModelComponents] ([MachineModelId]);
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MachineModelComponents");

            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Items]') AND name = 'ModelOrType')
ALTER TABLE [Items] DROP COLUMN [ModelOrType];");

            migrationBuilder.AlterColumn<decimal>(
                name: "Qty",
                table: "StepTemplateMaterials",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Qty",
                table: "StepExecMaterials",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);
        }
    }
}
