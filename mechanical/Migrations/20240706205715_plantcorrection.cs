using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class plantcorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NamePlant",
                table: "ProductionCapacities",
                newName: "PlantName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlantName",
                table: "ProductionCapacities",
                newName: "NamePlant");
        }
    }
}
