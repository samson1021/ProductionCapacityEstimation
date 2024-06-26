using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class phase3UpodatedPCECASesupdatednew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollateralEstimationFees_Cases_CaseId",
                table: "CollateralEstimationFees");

            migrationBuilder.DropForeignKey(
                name: "FK_FileUploads_ProductionCapacityEstimations_ProductionCapacityEstimationId",
                table: "FileUploads");

            migrationBuilder.DropForeignKey(
                name: "FK_FileUploads_ProductionCapacityEstimations_ProductionCapacityEstimationId1",
                table: "FileUploads");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductionCapacityEstimations_Cases_CaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropIndex(
                name: "IX_ProductionCapacityEstimations_CaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropIndex(
                name: "IX_FileUploads_ProductionCapacityEstimationId",
                table: "FileUploads");

            migrationBuilder.DropIndex(
                name: "IX_FileUploads_ProductionCapacityEstimationId1",
                table: "FileUploads");

            migrationBuilder.DropColumn(
                name: "CaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropColumn(
                name: "PCECaseId",
                table: "FileUploads");

            migrationBuilder.DropColumn(
                name: "ProductionCapacityEstimationId",
                table: "FileUploads");

            migrationBuilder.DropColumn(
                name: "ProductionCapacityEstimationId1",
                table: "FileUploads");

            migrationBuilder.RenameColumn(
                name: "CaseId",
                table: "CollateralEstimationFees",
                newName: "PCECaseId");

            migrationBuilder.RenameIndex(
                name: "IX_CollateralEstimationFees_CaseId",
                table: "CollateralEstimationFees",
                newName: "IX_CollateralEstimationFees_PCECaseId");

            migrationBuilder.AddColumn<Guid>(
                name: "PCEId",
                table: "FileUploads",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "FileUploads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacityEstimations_PCECaseId",
                table: "ProductionCapacityEstimations",
                column: "PCECaseId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUploads_PCEId",
                table: "FileUploads",
                column: "PCEId");

            migrationBuilder.AddForeignKey(
                name: "FK_CollateralEstimationFees_Cases_PCECaseId",
                table: "CollateralEstimationFees",
                column: "PCECaseId",
                principalTable: "Cases",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileUploads_ProductionCapacityEstimations_PCEId",
                table: "FileUploads",
                column: "PCEId",
                principalTable: "ProductionCapacityEstimations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionCapacityEstimations_Cases_PCECaseId",
                table: "ProductionCapacityEstimations",
                column: "PCECaseId",
                principalTable: "Cases",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollateralEstimationFees_Cases_PCECaseId",
                table: "CollateralEstimationFees");

            migrationBuilder.DropForeignKey(
                name: "FK_FileUploads_ProductionCapacityEstimations_PCEId",
                table: "FileUploads");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductionCapacityEstimations_Cases_PCECaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropIndex(
                name: "IX_ProductionCapacityEstimations_PCECaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropIndex(
                name: "IX_FileUploads_PCEId",
                table: "FileUploads");

            migrationBuilder.DropColumn(
                name: "PCEId",
                table: "FileUploads");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "FileUploads");

            migrationBuilder.RenameColumn(
                name: "PCECaseId",
                table: "CollateralEstimationFees",
                newName: "CaseId");

            migrationBuilder.RenameIndex(
                name: "IX_CollateralEstimationFees_PCECaseId",
                table: "CollateralEstimationFees",
                newName: "IX_CollateralEstimationFees_CaseId");

            migrationBuilder.AddColumn<Guid>(
                name: "CaseId",
                table: "ProductionCapacityEstimations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PCECaseId",
                table: "FileUploads",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductionCapacityEstimationId",
                table: "FileUploads",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductionCapacityEstimationId1",
                table: "FileUploads",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacityEstimations_CaseId",
                table: "ProductionCapacityEstimations",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUploads_ProductionCapacityEstimationId",
                table: "FileUploads",
                column: "ProductionCapacityEstimationId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUploads_ProductionCapacityEstimationId1",
                table: "FileUploads",
                column: "ProductionCapacityEstimationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CollateralEstimationFees_Cases_CaseId",
                table: "CollateralEstimationFees",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileUploads_ProductionCapacityEstimations_ProductionCapacityEstimationId",
                table: "FileUploads",
                column: "ProductionCapacityEstimationId",
                principalTable: "ProductionCapacityEstimations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileUploads_ProductionCapacityEstimations_ProductionCapacityEstimationId1",
                table: "FileUploads",
                column: "ProductionCapacityEstimationId1",
                principalTable: "ProductionCapacityEstimations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionCapacityEstimations_Cases_CaseId",
                table: "ProductionCapacityEstimations",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "Id");
        }
    }
}
