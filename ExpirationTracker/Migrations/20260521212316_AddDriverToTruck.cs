using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpirationTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverToTruck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DriverId",
                table: "Trucks",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Trucks",
                keyColumn: "Id",
                keyValue: 1,
                column: "DriverId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Trucks",
                keyColumn: "Id",
                keyValue: 2,
                column: "DriverId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Trucks",
                keyColumn: "Id",
                keyValue: 3,
                column: "DriverId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Trucks",
                keyColumn: "Id",
                keyValue: 4,
                column: "DriverId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Trucks",
                keyColumn: "Id",
                keyValue: 5,
                column: "DriverId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Trucks",
                keyColumn: "Id",
                keyValue: 6,
                column: "DriverId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Trucks",
                keyColumn: "Id",
                keyValue: 7,
                column: "DriverId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Trucks",
                keyColumn: "Id",
                keyValue: 8,
                column: "DriverId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Trucks",
                keyColumn: "Id",
                keyValue: 9,
                column: "DriverId",
                value: 7);

            migrationBuilder.Sql(@"
                UPDATE [Trucks]
                SET [DriverId] = NULL
                WHERE [DriverId] IS NOT NULL
                  AND [DriverId] NOT IN (SELECT [Id] FROM [Drivers]);
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_DriverId",
                table: "Trucks",
                column: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_Drivers_DriverId",
                table: "Trucks",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_Drivers_DriverId",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_DriverId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "Trucks");
        }
    }
}
