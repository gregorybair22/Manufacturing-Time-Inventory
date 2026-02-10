using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManufacturingTimeTracking.Migrations
{
    /// <inheritdoc />
    public partial class OrderPickLines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create OrderPickLines only if missing (may already exist from Program.cs startup)
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[OrderPickLines]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[OrderPickLines] (
        [Id] int NOT NULL IDENTITY,
        [BuildOrderId] int NOT NULL,
        [ItemId] int NOT NULL,
        [QuantityRequired] int NOT NULL,
        [QuantityPicked] int NOT NULL DEFAULT 0,
        CONSTRAINT [PK_OrderPickLines] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrderPickLines_BuildOrders_BuildOrderId] FOREIGN KEY ([BuildOrderId]) REFERENCES [dbo].[BuildOrders] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderPickLines_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([Id]) ON DELETE NO ACTION
    );
    CREATE INDEX [IX_OrderPickLines_BuildOrderId] ON [dbo].[OrderPickLines] ([BuildOrderId]);
    CREATE UNIQUE INDEX [IX_OrderPickLines_BuildOrderId_ItemId] ON [dbo].[OrderPickLines] ([BuildOrderId], [ItemId]);
    CREATE INDEX [IX_OrderPickLines_ItemId] ON [dbo].[OrderPickLines] ([ItemId]);
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderPickLines");
        }
    }
}
