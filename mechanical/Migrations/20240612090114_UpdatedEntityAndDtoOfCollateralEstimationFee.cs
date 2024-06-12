using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEntityAndDtoOfCollateralEstimationFee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionCapacityEstimations_Cases_CaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropTable(
                name: "CollateralDetails");

            migrationBuilder.DropIndex(
                name: "IX_ProductionCapacityEstimations_CaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropColumn(
                name: "CaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.CreateTable(
                name: "CollateralEstimationFees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CollateralClass = table.Column<int>(type: "int", nullable: false),
                    CollateralCategory = table.Column<int>(type: "int", nullable: false),
                    UnitOfMeasure = table.Column<int>(type: "int", nullable: false),
                    EstimationFeePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollateralEstimationFees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollateralEstimationFees_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollateralEstimationFees_CaseId",
                table: "CollateralEstimationFees",
                column: "CaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollateralEstimationFees");

            migrationBuilder.AddColumn<Guid>(
                name: "CaseId",
                table: "ProductionCapacityEstimations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CollateralDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Class = table.Column<int>(type: "int", nullable: false),
                    EstimationFeePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsCommitted = table.Column<bool>(type: "bit", nullable: false),
                    IsValidated = table.Column<bool>(type: "bit", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitOfMeasure = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollateralDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollateralDetails_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacityEstimations_CaseId",
                table: "ProductionCapacityEstimations",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CollateralDetails_CaseId",
                table: "CollateralDetails",
                column: "CaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionCapacityEstimations_Cases_CaseId",
                table: "ProductionCapacityEstimations",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "Id");
        }
    }
}
