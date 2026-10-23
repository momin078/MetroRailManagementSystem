using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MetroRailManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class SeedStationsAndTrains : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "Code", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "UN", true, "Uttara North" },
                    { 2, "UC", true, "Uttara Center" },
                    { 3, "AG", true, "Agargaon" }
                });

            migrationBuilder.InsertData(
                table: "Trains",
                columns: new[] { "Id", "AvailableSeats", "IsActive", "Name", "TotalSeats" },
                values: new object[,]
                {
                    { 1, 200, true, "Metro Express 1", 200 },
                    { 2, 300, true, "Metro Express 2", 300 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Trains",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Trains",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
