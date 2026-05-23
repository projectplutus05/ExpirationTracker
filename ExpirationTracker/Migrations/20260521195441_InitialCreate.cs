using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExpirationTracker.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DriverName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TruckNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DotInspectionExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TruckTagExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IrpExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhysicalExpiry = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trailers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignedTo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrailerNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HasHeadboard = table.Column<bool>(type: "bit", nullable: false),
                    Year = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Make = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Vin = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TagNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TagExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TagNotes = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AnnualInspExpiry = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trailers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "DotInspectionExpiry", "DriverName", "DriverNumber", "IrpExpiry", "PhysicalExpiry", "TruckNumber", "TruckTagExpiry" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Buck", "0815", new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "5800", new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2027, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Darron", "7065", null, null, "5003", null },
                    { 3, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jerome", "1190", new DateTime(2027, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "3439", new DateTime(2027, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2027, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jerome", null, new DateTime(2027, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "3440", new DateTime(2027, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2026, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chris G", "1960", new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "5737", new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, new DateTime(2026, 11, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chris G", null, new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "5736", new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, new DateTime(2026, 8, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jeff", "4366", new DateTime(2027, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "6195", new DateTime(2027, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, new DateTime(2026, 8, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Q", "9271", new DateTime(2026, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "6181", new DateTime(2026, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, new DateTime(2027, 3, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Freddy", "4057", new DateTime(2026, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "455", new DateTime(2026, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Trailers",
                columns: new[] { "Id", "AnnualInspExpiry", "AssignedTo", "HasHeadboard", "Make", "Model", "TagExpiry", "TagNotes", "TagNumber", "TrailerNumber", "Vin", "Year" },
                values: new object[,]
                {
                    { 1, new DateTime(2027, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Buck", false, "Transcraft", "554C Eagle II", new DateTime(2026, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "1A0BUM2", "7432", "1TTF482CXL3196914", "2020" },
                    { 2, new DateTime(2027, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Darron", true, "Transcraft", "554C Eagle II", new DateTime(2026, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "1A0BUHX", "5863", "1TTF482C3L3196866", "2020" },
                    { 3, new DateTime(2027, 1, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jerome", false, "Transcraft", "554C Eagle II", new DateTime(2026, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "1A0BUM1", "5860", "1TTF482CXJ3060408", "2018" },
                    { 4, new DateTime(2026, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chris G", false, "Transcraft", "48' Flatbed", new DateTime(2026, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "62AOMBH", "943", "1TTF482C3F3895967", "2015" },
                    { 5, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jeff", false, "Transcraft", "554C Eagle II", new DateTime(2026, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "1AOBUHY", "5862", "1TTF482C0K3142116", "2019" },
                    { 6, new DateTime(2027, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Q", true, "Transcraft", "554C Eagle II", null, "Perm", "A004585", "5854", "1TTF482C1H3027890", "2017" },
                    { 7, new DateTime(2027, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Freddy", true, "Transcraft", "48' Flatbed", new DateTime(2026, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "1A0BUHE", "7201", "1TTF482C5C3576713", "2012" },
                    { 8, new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rented", false, "Transcraft", "554C Eagle II", new DateTime(2026, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "1A0BUM0", "5861", "1TTF482C4K3133001", "2019" },
                    { 9, new DateTime(2027, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Transcraft", "554C Eagle II", null, "Perm", "A138145", "60852", "1TTF482C2F3873328", "2015" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "Trailers");
        }
    }
}
