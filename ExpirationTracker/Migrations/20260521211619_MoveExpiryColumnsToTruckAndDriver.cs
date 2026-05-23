using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExpirationTracker.Migrations
{
    /// <inheritdoc />
    public partial class MoveExpiryColumnsToTruckAndDriver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DotInspectionExpiry",
                table: "Trucks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IrpExpiry",
                table: "Trucks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TruckTagExpiry",
                table: "Trucks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseExpiry",
                table: "Drivers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PhysicalExpiry",
                table: "Drivers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE t
                SET t.[DotInspectionExpiry] = src.[DotInspectionExpiry],
                    t.[TruckTagExpiry] = src.[TruckTagExpiry],
                    t.[IrpExpiry] = src.[IrpExpiry]
                FROM [Trucks] t
                INNER JOIN (
                    SELECT e.[TruckId],
                           MAX(CASE WHEN e.[ExpirationType] = N'DOT Inspection' THEN e.[ExpiryOn] END) AS [DotInspectionExpiry],
                           MAX(CASE WHEN e.[ExpirationType] = N'Truck Tag' THEN e.[ExpiryOn] END) AS [TruckTagExpiry],
                           MAX(CASE WHEN e.[ExpirationType] = N'IRP' THEN e.[ExpiryOn] END) AS [IrpExpiry]
                    FROM [ExpiryDates] e
                    GROUP BY e.[TruckId]
                ) src ON src.[TruckId] = t.[Id];
            ");

            migrationBuilder.Sql(@"
                UPDATE d
                SET d.[PhysicalExpiry] = src.[PhysicalExpiry],
                    d.[LicenseExpiry] = src.[LicenseExpiry]
                FROM [Drivers] d
                INNER JOIN (
                    SELECT e.[DriverId],
                           MAX(CASE WHEN e.[ExpirationType] = N'Physical' THEN e.[ExpiryOn] END) AS [PhysicalExpiry],
                           MAX(CASE WHEN e.[ExpirationType] = N'License' THEN e.[ExpiryOn] END) AS [LicenseExpiry]
                    FROM [ExpiryDates] e
                    GROUP BY e.[DriverId]
                ) src ON src.[DriverId] = d.[Id];
            ");

            migrationBuilder.DropTable(
                name: "ExpiryDates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DotInspectionExpiry",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "IrpExpiry",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "TruckTagExpiry",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "LicenseExpiry",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "PhysicalExpiry",
                table: "Drivers");

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

            migrationBuilder.InsertData(
                table: "ExpiryDates",
                columns: new[] { "Id", "DriverId", "ExpirationType", "ExpiryOn", "TruckId" },
                values: new object[,]
                {
                    { 1, 1, "DOT Inspection", new DateTime(2026, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 1, "Truck Tag", new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 3, 1, "IRP", new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 4, 1, "Physical", new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 5, 2, "DOT Inspection", new DateTime(2027, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 6, 3, "DOT Inspection", new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 7, 3, "Truck Tag", new DateTime(2027, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 8, 3, "IRP", new DateTime(2027, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 9, 3, "Physical", new DateTime(2027, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 10, 3, "DOT Inspection", new DateTime(2027, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 11, 3, "Truck Tag", new DateTime(2027, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 12, 3, "IRP", new DateTime(2027, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 13, 3, "Physical", new DateTime(2027, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 14, 4, "DOT Inspection", new DateTime(2026, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 5 },
                    { 15, 4, "Truck Tag", new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 5 },
                    { 16, 4, "IRP", new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 5 },
                    { 17, 4, "Physical", new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 5 },
                    { 18, 4, "DOT Inspection", new DateTime(2026, 11, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 6 },
                    { 19, 4, "Truck Tag", new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 6 },
                    { 20, 4, "IRP", new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 6 },
                    { 21, 4, "Physical", new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 6 },
                    { 22, 5, "DOT Inspection", new DateTime(2026, 8, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 7 },
                    { 23, 5, "Truck Tag", new DateTime(2027, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 7 },
                    { 24, 5, "IRP", new DateTime(2027, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 7 },
                    { 25, 5, "Physical", new DateTime(2027, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 7 },
                    { 26, 6, "DOT Inspection", new DateTime(2026, 8, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 8 },
                    { 27, 6, "Truck Tag", new DateTime(2026, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 8 },
                    { 28, 6, "IRP", new DateTime(2026, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 8 },
                    { 29, 6, "Physical", new DateTime(2026, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 8 },
                    { 30, 7, "DOT Inspection", new DateTime(2027, 3, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 9 },
                    { 31, 7, "Truck Tag", new DateTime(2026, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 9 },
                    { 32, 7, "IRP", new DateTime(2026, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 9 },
                    { 33, 7, "Physical", new DateTime(2026, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 9 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpiryDates_DriverId",
                table: "ExpiryDates",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpiryDates_TruckId",
                table: "ExpiryDates",
                column: "TruckId");
        }
    }
}
