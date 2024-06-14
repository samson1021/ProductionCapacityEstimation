using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class Phase2UpdatedEntityAndDTOsOptional2Per : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PerDayProduction",
                table: "ProductionCapacityEstimations",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PerMonthProduction",
                table: "ProductionCapacityEstimations",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PerShiftProduction",
                table: "ProductionCapacityEstimations",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PerYearProduction",
                table: "ProductionCapacityEstimations",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerDayProduction",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropColumn(
                name: "PerMonthProduction",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropColumn(
                name: "PerShiftProduction",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropColumn(
                name: "PerYearProduction",
                table: "ProductionCapacityEstimations");
        }
    }
}
