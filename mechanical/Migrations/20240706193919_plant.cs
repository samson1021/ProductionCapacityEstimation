using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class plant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfInspection",
                table: "ProductionCapacities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "ProductionCapacities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NamePlant",
                table: "ProductionCapacities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ObsolescenceStatus",
                table: "ProductionCapacities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlantDepreciationRate",
                table: "ProductionCapacities",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfInspection",
                table: "ProductionCapacities");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "ProductionCapacities");

            migrationBuilder.DropColumn(
                name: "NamePlant",
                table: "ProductionCapacities");

            migrationBuilder.DropColumn(
                name: "ObsolescenceStatus",
                table: "ProductionCapacities");

            migrationBuilder.DropColumn(
                name: "PlantDepreciationRate",
                table: "ProductionCapacities");
        }
    }
}
