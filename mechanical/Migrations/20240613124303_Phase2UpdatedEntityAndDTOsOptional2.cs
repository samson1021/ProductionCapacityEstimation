using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class Phase2UpdatedEntityAndDTOsOptional2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollateralEstimationFees_Cases_CaseId",
                table: "CollateralEstimationFees");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductionCapacityEstimations_Cases_CaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.AlterColumn<Guid>(
                name: "CaseId",
                table: "ProductionCapacityEstimations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CaseId",
                table: "CollateralEstimationFees",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_CollateralEstimationFees_Cases_CaseId",
                table: "CollateralEstimationFees",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionCapacityEstimations_Cases_CaseId",
                table: "ProductionCapacityEstimations",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollateralEstimationFees_Cases_CaseId",
                table: "CollateralEstimationFees");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductionCapacityEstimations_Cases_CaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.AlterColumn<Guid>(
                name: "CaseId",
                table: "ProductionCapacityEstimations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CaseId",
                table: "CollateralEstimationFees",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CollateralEstimationFees_Cases_CaseId",
                table: "CollateralEstimationFees",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionCapacityEstimations_Cases_CaseId",
                table: "ProductionCapacityEstimations",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
