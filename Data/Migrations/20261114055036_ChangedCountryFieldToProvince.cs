using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evently.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedCountryFieldToProvince : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Country",
                table: "AspNetUsers",
                newName: "Province");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Province",
                table: "AspNetUsers",
                newName: "Country");
        }
    }
}
