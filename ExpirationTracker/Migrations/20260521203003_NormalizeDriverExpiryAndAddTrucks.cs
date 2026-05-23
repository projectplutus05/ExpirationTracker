using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExpirationTracker.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeDriverExpiryAndAddTrucks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trucks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TruckNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Make = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModelYear = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    Vin = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trucks", x => x.Id);
                });

            migrationBuilder.Sql(@"
                INSERT INTO [Trucks] ([TruckNo])
                SELECT DISTINCT [TruckNumber]
                FROM [Drivers]
                WHERE [TruckNumber] IS NOT NULL AND LTRIM(RTRIM([TruckNumber])) <> '';
            ");

            migrationBuilder.CreateTable(
                name: "ExpiryDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverId = table.Column<int>(type: "int", nullable: false),
                    TruckId = table.Column<int>(type: "int", nullable: false),
                    ExpirationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExpiryOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpiryDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpiryDates_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpiryDates_Trucks_TruckId",
                        column: x => x.TruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql(@"
                INSERT INTO [ExpiryDates] ([DriverId], [TruckId], [ExpirationType], [ExpiryOn])
                SELECT d.[Id], t.[Id], N'DOT Inspection', d.[DotInspectionExpiry]
                FROM [Drivers] d
                INNER JOIN [Trucks] t ON t.[TruckNo] = d.[TruckNumber]
                WHERE d.[DotInspectionExpiry] IS NOT NULL

                UNION ALL

                SELECT d.[Id], t.[Id], N'Truck Tag', d.[TruckTagExpiry]
                FROM [Drivers] d
                INNER JOIN [Trucks] t ON t.[TruckNo] = d.[TruckNumber]
                WHERE d.[TruckTagExpiry] IS NOT NULL

                UNION ALL

                SELECT d.[Id], t.[Id], N'IRP', d.[IrpExpiry]
                FROM [Drivers] d
                INNER JOIN [Trucks] t ON t.[TruckNo] = d.[TruckNumber]
                WHERE d.[IrpExpiry] IS NOT NULL

                UNION ALL

                SELECT d.[Id], t.[Id], N'Physical', d.[PhysicalExpiry]
                FROM [Drivers] d
                INNER JOIN [Trucks] t ON t.[TruckNo] = d.[TruckNumber]
                WHERE d.[PhysicalExpiry] IS NOT NULL;
            ");

            migrationBuilder.DropColumn(
                name: "DotInspectionExpiry",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "IrpExpiry",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "PhysicalExpiry",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "TruckNumber",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "TruckTagExpiry",
                table: "Drivers");

            migrationBuilder.CreateIndex(
                name: "IX_ExpiryDates_DriverId",
                table: "ExpiryDates",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpiryDates_TruckId",
                table: "ExpiryDates",
                column: "TruckId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpiryDates");

            migrationBuilder.DropTable(
                name: "Trucks");

            migrationBuilder.AddColumn<DateTime>(
                name: "DotInspectionExpiry",
                table: "Drivers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IrpExpiry",
                table: "Drivers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PhysicalExpiry",
                table: "Drivers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TruckNumber",
                table: "Drivers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TruckTagExpiry",
                table: "Drivers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE d
                SET d.[DotInspectionExpiry] = src.[DotInspectionExpiry],
                    d.[TruckTagExpiry] = src.[TruckTagExpiry],
                    d.[IrpExpiry] = src.[IrpExpiry],
                    d.[PhysicalExpiry] = src.[PhysicalExpiry],
                    d.[TruckNumber] = src.[TruckNumber]
                FROM [Drivers] d
                INNER JOIN (
                    SELECT e.[DriverId],
                           t.[TruckNo] AS [TruckNumber],
                           MAX(CASE WHEN e.[ExpirationType] = N'DOT Inspection' THEN e.[ExpiryOn] END) AS [DotInspectionExpiry],
                           MAX(CASE WHEN e.[ExpirationType] = N'Truck Tag' THEN e.[ExpiryOn] END) AS [TruckTagExpiry],
                           MAX(CASE WHEN e.[ExpirationType] = N'IRP' THEN e.[ExpiryOn] END) AS [IrpExpiry],
                           MAX(CASE WHEN e.[ExpirationType] = N'Physical' THEN e.[ExpiryOn] END) AS [PhysicalExpiry]
                    FROM [ExpiryDates] e
                    INNER JOIN [Trucks] t ON t.[Id] = e.[TruckId]
                    GROUP BY e.[DriverId], t.[TruckNo]
                ) src ON src.[DriverId] = d.[Id];
            ");
        }
    }
}
