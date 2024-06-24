using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class phase3UpodatedCASes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionCapacityEstimations_Cases_PCECaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropIndex(
                name: "IX_ProductionCapacityEstimations_PCECaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.AddColumn<Guid>(
                name: "CaseId",
                table: "ProductionCapacityEstimations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacityEstimations_CaseId",
                table: "ProductionCapacityEstimations",
                column: "CaseId");

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
                name: "FK_ProductionCapacityEstimations_Cases_CaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropIndex(
                name: "IX_ProductionCapacityEstimations_CaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropColumn(
                name: "CaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacityEstimations_PCECaseId",
                table: "ProductionCapacityEstimations",
                column: "PCECaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionCapacityEstimations_Cases_PCECaseId",
                table: "ProductionCapacityEstimations",
                column: "PCECaseId",
                principalTable: "Cases",
                principalColumn: "Id");
        }
    }
}
