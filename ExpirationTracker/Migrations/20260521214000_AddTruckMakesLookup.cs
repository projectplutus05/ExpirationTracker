using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExpirationTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddTruckMakesLookup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TruckMakes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TruckMakes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TruckMakes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Freightliner" },
                    { 2, "Peterbilt" },
                    { 3, "Volvo" }
                });

            migrationBuilder.AddColumn<int>(
                name: "TruckMakeId",
                table: "Trucks",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE t
                SET t.TruckMakeId = tm.Id
                FROM [Trucks] t
                LEFT JOIN [TruckMakes] tm
                    ON UPPER(LTRIM(RTRIM(ISNULL(t.[Make], '')))) = UPPER(tm.[Name]);

                UPDATE [Trucks]
                SET [TruckMakeId] = 1
                WHERE [TruckMakeId] IS NULL;
            ");

            migrationBuilder.AlterColumn<int>(
                name: "TruckMakeId",
                table: "Trucks",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "Make",
                table: "Trucks");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_TruckMakeId",
                table: "Trucks",
                column: "TruckMakeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_TruckMakes_TruckMakeId",
                table: "Trucks",
                column: "TruckMakeId",
                principalTable: "TruckMakes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_TruckMakes_TruckMakeId",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_TruckMakeId",
                table: "Trucks");

            migrationBuilder.AddColumn<string>(
                name: "Make",
                table: "Trucks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE t
                SET t.[Make] = tm.[Name]
                FROM [Trucks] t
                INNER JOIN [TruckMakes] tm ON t.[TruckMakeId] = tm.[Id];
            ");

            migrationBuilder.DropColumn(
                name: "TruckMakeId",
                table: "Trucks");

            migrationBuilder.DropTable(
                name: "TruckMakes");
        }
    }
}
