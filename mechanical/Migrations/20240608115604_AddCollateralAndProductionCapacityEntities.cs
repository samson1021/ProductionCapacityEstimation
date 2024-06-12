using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class AddCollateralAndProductionCapacityEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductionCapacityEstimationId",
                table: "UploadFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductionCapacityEstimationId1",
                table: "UploadFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CollateralDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Class = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    UnitOfMeasure = table.Column<int>(type: "int", nullable: false),
                    EstimationFeePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    IsValidated = table.Column<bool>(type: "bit", nullable: false),
                    IsCommitted = table.Column<bool>(type: "bit", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ProductionCapacityEstimations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeOfOutput = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhaseOfOutput = table.Column<int>(type: "int", nullable: false),
                    ProductionLineOrEquipmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryOfOrigin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftsPerDay = table.Column<int>(type: "int", nullable: true),
                    WorkingDaysPerMonth = table.Column<int>(type: "int", nullable: true),
                    EffectiveProductionHour = table.Column<int>(type: "int", nullable: true),
                    EffectiveProductionHourType = table.Column<int>(type: "int", nullable: true),
                    UnitOfProduction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductionMeasurement = table.Column<int>(type: "int", nullable: false),
                    EstimatedProductionCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BottleneckProductionLineCapacity = table.Column<int>(type: "int", nullable: true),
                    OverallActualCurrentPlantCapacity = table.Column<int>(type: "int", nullable: true),
                    TimeConsumedToCheckStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TimeConsumedToCheckEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TechnicalObsolescenceStatus = table.Column<int>(type: "int", nullable: false),
                    DepreciationRateApplied = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Discrepancies = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DesignProductionCapacity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttainableProductionCapacity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlaceOfInspection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FactorsAffectingProductionCapacity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineFunctionalityStatus = table.Column<int>(type: "int", nullable: false),
                    ReasonForNotFunctional = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SurveyRemark = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaseReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionCapacityEstimations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionCapacityEstimations_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftHour",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductionCapacityEstimationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftHour", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftHour_ProductionCapacityEstimations_ProductionCapacityEstimationId",
                        column: x => x.ProductionCapacityEstimationId,
                        principalTable: "ProductionCapacityEstimations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UploadFiles_ProductionCapacityEstimationId",
                table: "UploadFiles",
                column: "ProductionCapacityEstimationId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadFiles_ProductionCapacityEstimationId1",
                table: "UploadFiles",
                column: "ProductionCapacityEstimationId1");

            migrationBuilder.CreateIndex(
                name: "IX_CollateralDetails_CaseId",
                table: "CollateralDetails",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacityEstimations_CaseId",
                table: "ProductionCapacityEstimations",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftHour_ProductionCapacityEstimationId",
                table: "ShiftHour",
                column: "ProductionCapacityEstimationId");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadFiles_ProductionCapacityEstimations_ProductionCapacityEstimationId",
                table: "UploadFiles",
                column: "ProductionCapacityEstimationId",
                principalTable: "ProductionCapacityEstimations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadFiles_ProductionCapacityEstimations_ProductionCapacityEstimationId1",
                table: "UploadFiles",
                column: "ProductionCapacityEstimationId1",
                principalTable: "ProductionCapacityEstimations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadFiles_ProductionCapacityEstimations_ProductionCapacityEstimationId",
                table: "UploadFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadFiles_ProductionCapacityEstimations_ProductionCapacityEstimationId1",
                table: "UploadFiles");

            migrationBuilder.DropTable(
                name: "CollateralDetails");

            migrationBuilder.DropTable(
                name: "ShiftHour");

            migrationBuilder.DropTable(
                name: "ProductionCapacityEstimations");

            migrationBuilder.DropIndex(
                name: "IX_UploadFiles_ProductionCapacityEstimationId",
                table: "UploadFiles");

            migrationBuilder.DropIndex(
                name: "IX_UploadFiles_ProductionCapacityEstimationId1",
                table: "UploadFiles");

            migrationBuilder.DropColumn(
                name: "ProductionCapacityEstimationId",
                table: "UploadFiles");

            migrationBuilder.DropColumn(
                name: "ProductionCapacityEstimationId1",
                table: "UploadFiles");
        }
    }
}
