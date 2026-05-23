using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpirationTracker.Migrations
{
    /// <inheritdoc />
    public partial class RequireTruckDriver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_Drivers_DriverId",
                table: "Trucks");

            migrationBuilder.Sql(@"
                DECLARE @fallbackDriverId INT;
                SELECT @fallbackDriverId = MIN([Id]) FROM [Drivers];

                UPDATE [Trucks]
                SET [DriverId] = @fallbackDriverId
                WHERE [DriverId] IS NULL;
            ");

            migrationBuilder.AlterColumn<int>(
                name: "DriverId",
                table: "Trucks",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_Drivers_DriverId",
                table: "Trucks",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_Drivers_DriverId",
                table: "Trucks");

            migrationBuilder.AlterColumn<int>(
                name: "DriverId",
                table: "Trucks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_Drivers_DriverId",
                table: "Trucks",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
