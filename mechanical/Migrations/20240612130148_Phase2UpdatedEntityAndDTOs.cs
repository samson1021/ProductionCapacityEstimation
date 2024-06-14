using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class Phase2UpdatedEntityAndDTOs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionCapacitySchedule_ProductionCapacityEstimations_ProductionCapacityEstimationId",
                table: "ProductionCapacitySchedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductionCapacitySchedule",
                table: "ProductionCapacitySchedule");

            migrationBuilder.RenameTable(
                name: "ProductionCapacitySchedule",
                newName: "ProductionCapacitySchedules");

            migrationBuilder.RenameIndex(
                name: "IX_ProductionCapacitySchedule_ProductionCapacityEstimationId",
                table: "ProductionCapacitySchedules",
                newName: "IX_ProductionCapacitySchedules_ProductionCapacityEstimationId");

            migrationBuilder.AddColumn<Guid>(
                name: "CaseId",
                table: "ProductionCapacityEstimations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ProductionCapacityEstimations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "CollateralEstimationFees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductionCapacitySchedules",
                table: "ProductionCapacitySchedules",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacityEstimations_CaseId",
                table: "ProductionCapacityEstimations",
                column: "CaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionCapacityEstimations_Cases_CaseId",
                table: "ProductionCapacityEstimations",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionCapacitySchedules_ProductionCapacityEstimations_ProductionCapacityEstimationId",
                table: "ProductionCapacitySchedules",
                column: "ProductionCapacityEstimationId",
                principalTable: "ProductionCapacityEstimations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionCapacityEstimations_Cases_CaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductionCapacitySchedules_ProductionCapacityEstimations_ProductionCapacityEstimationId",
                table: "ProductionCapacitySchedules");

            migrationBuilder.DropIndex(
                name: "IX_ProductionCapacityEstimations_CaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductionCapacitySchedules",
                table: "ProductionCapacitySchedules");

            migrationBuilder.DropColumn(
                name: "CaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "CollateralEstimationFees");

            migrationBuilder.RenameTable(
                name: "ProductionCapacitySchedules",
                newName: "ProductionCapacitySchedule");

            migrationBuilder.RenameIndex(
                name: "IX_ProductionCapacitySchedules_ProductionCapacityEstimationId",
                table: "ProductionCapacitySchedule",
                newName: "IX_ProductionCapacitySchedule_ProductionCapacityEstimationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductionCapacitySchedule",
                table: "ProductionCapacitySchedule",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionCapacitySchedule_ProductionCapacityEstimations_ProductionCapacityEstimationId",
                table: "ProductionCapacitySchedule",
                column: "ProductionCapacityEstimationId",
                principalTable: "ProductionCapacityEstimations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
