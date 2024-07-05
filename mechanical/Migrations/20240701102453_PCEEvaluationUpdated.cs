using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class PCEEvaluationUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollateralEstimationFees_ProductionCapacityEstimations_PCEId",
                table: "CollateralEstimationFees");

            migrationBuilder.DropForeignKey(
                name: "FK_FileUploads_ProductionCapacityEstimations_PCEId",
                table: "FileUploads");

            migrationBuilder.DropForeignKey(
                name: "FK_TimePeriod_ProductionCapacityEstimations_ProductionCapacityEstimationId",
                table: "TimePeriod");

            migrationBuilder.DropTable(
                name: "ProductionCapacityEstimations");

            migrationBuilder.RenameColumn(
                name: "ProductionCapacityEstimationId",
                table: "TimePeriod",
                newName: "PCEEvaluationId");

            migrationBuilder.RenameIndex(
                name: "IX_TimePeriod_ProductionCapacityEstimationId",
                table: "TimePeriod",
                newName: "IX_TimePeriod_PCEEvaluationId");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "FileUploads",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "PCEId",
                table: "FileUploads",
                newName: "PCEEId");

            migrationBuilder.RenameIndex(
                name: "IX_FileUploads_PCEId",
                table: "FileUploads",
                newName: "IX_FileUploads_PCEEId");

            migrationBuilder.RenameColumn(
                name: "PCEId",
                table: "CollateralEstimationFees",
                newName: "PCEEId");

            migrationBuilder.RenameIndex(
                name: "IX_CollateralEstimationFees_PCEId",
                table: "CollateralEstimationFees",
                newName: "IX_CollateralEstimationFees_PCEEId");

            migrationBuilder.CreateTable(
                name: "PCEEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PCECaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductionLineOrEquipmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutputType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutputPhase = table.Column<int>(type: "int", nullable: true),
                    OriginCountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductionUnit = table.Column<int>(type: "int", nullable: true),
                    WorkingDaysPerMonth = table.Column<int>(type: "int", nullable: true),
                    ShiftsPerDay = table.Column<int>(type: "int", nullable: true),
                    ProductionMeasurement = table.Column<int>(type: "int", nullable: true),
                    EstimatedProductionCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BottleneckProductionLineCapacity = table.Column<int>(type: "int", nullable: true),
                    OverallActualCurrentPlantCapacity = table.Column<int>(type: "int", nullable: true),
                    TimeConsumedToCheckId = table.Column<int>(type: "int", nullable: true),
                    TechnicalObsolescenceStatus = table.Column<int>(type: "int", nullable: true),
                    DepreciationRateApplied = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Discrepancies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EffectiveProductionHourType = table.Column<int>(type: "int", nullable: true),
                    EffectiveProductionHour = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DesignProductionCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AttainableProductionCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ActualProductionCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FactorsAffectingProductionCapacity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MachineFunctionalityStatus = table.Column<int>(type: "int", nullable: true),
                    MachineFunctionalityReason = table.Column<int>(type: "int", nullable: true),
                    OtherMachineFunctionalityReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InspectionPlace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SurveyRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCEEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PCEEvaluations_DateTimePeriod_TimeConsumedToCheckId",
                        column: x => x.TimeConsumedToCheckId,
                        principalTable: "DateTimePeriod",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PCEEvaluations_PCECases_PCECaseId",
                        column: x => x.PCECaseId,
                        principalTable: "PCECases",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PCEEvaluations_PCECaseId",
                table: "PCEEvaluations",
                column: "PCECaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PCEEvaluations_TimeConsumedToCheckId",
                table: "PCEEvaluations",
                column: "TimeConsumedToCheckId");

            migrationBuilder.AddForeignKey(
                name: "FK_CollateralEstimationFees_PCEEvaluations_PCEEId",
                table: "CollateralEstimationFees",
                column: "PCEEId",
                principalTable: "PCEEvaluations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileUploads_PCEEvaluations_PCEEId",
                table: "FileUploads",
                column: "PCEEId",
                principalTable: "PCEEvaluations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimePeriod_PCEEvaluations_PCEEvaluationId",
                table: "TimePeriod",
                column: "PCEEvaluationId",
                principalTable: "PCEEvaluations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollateralEstimationFees_PCEEvaluations_PCEEId",
                table: "CollateralEstimationFees");

            migrationBuilder.DropForeignKey(
                name: "FK_FileUploads_PCEEvaluations_PCEEId",
                table: "FileUploads");

            migrationBuilder.DropForeignKey(
                name: "FK_TimePeriod_PCEEvaluations_PCEEvaluationId",
                table: "TimePeriod");

            migrationBuilder.DropTable(
                name: "PCEEvaluations");

            migrationBuilder.RenameColumn(
                name: "PCEEvaluationId",
                table: "TimePeriod",
                newName: "ProductionCapacityEstimationId");

            migrationBuilder.RenameIndex(
                name: "IX_TimePeriod_PCEEvaluationId",
                table: "TimePeriod",
                newName: "IX_TimePeriod_ProductionCapacityEstimationId");

            migrationBuilder.RenameColumn(
                name: "PCEEId",
                table: "FileUploads",
                newName: "PCEId");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "FileUploads",
                newName: "Type");

            migrationBuilder.RenameIndex(
                name: "IX_FileUploads_PCEEId",
                table: "FileUploads",
                newName: "IX_FileUploads_PCEId");

            migrationBuilder.RenameColumn(
                name: "PCEEId",
                table: "CollateralEstimationFees",
                newName: "PCEId");

            migrationBuilder.RenameIndex(
                name: "IX_CollateralEstimationFees_PCEEId",
                table: "CollateralEstimationFees",
                newName: "IX_CollateralEstimationFees_PCEId");

            migrationBuilder.CreateTable(
                name: "ProductionCapacityEstimations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PCECaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TimeConsumedToCheckId = table.Column<int>(type: "int", nullable: true),
                    ActualProductionCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AttainableProductionCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BottleneckProductionLineCapacity = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepreciationRateApplied = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DesignProductionCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Discrepancies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EffectiveProductionHour = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EffectiveProductionHourType = table.Column<int>(type: "int", nullable: true),
                    EstimatedProductionCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FactorsAffectingProductionCapacity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InspectionPlace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MachineFunctionalityReason = table.Column<int>(type: "int", nullable: true),
                    MachineFunctionalityStatus = table.Column<int>(type: "int", nullable: true),
                    OriginCountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherMachineFunctionalityReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutputPhase = table.Column<int>(type: "int", nullable: true),
                    OutputType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverallActualCurrentPlantCapacity = table.Column<int>(type: "int", nullable: true),
                    ProductionLineOrEquipmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductionMeasurement = table.Column<int>(type: "int", nullable: true),
                    ProductionUnit = table.Column<int>(type: "int", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShiftsPerDay = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    SurveyRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TechnicalObsolescenceStatus = table.Column<int>(type: "int", nullable: true),
                    WorkingDaysPerMonth = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionCapacityEstimations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionCapacityEstimations_DateTimePeriod_TimeConsumedToCheckId",
                        column: x => x.TimeConsumedToCheckId,
                        principalTable: "DateTimePeriod",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductionCapacityEstimations_PCECases_PCECaseId",
                        column: x => x.PCECaseId,
                        principalTable: "PCECases",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacityEstimations_PCECaseId",
                table: "ProductionCapacityEstimations",
                column: "PCECaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacityEstimations_TimeConsumedToCheckId",
                table: "ProductionCapacityEstimations",
                column: "TimeConsumedToCheckId");

            migrationBuilder.AddForeignKey(
                name: "FK_CollateralEstimationFees_ProductionCapacityEstimations_PCEId",
                table: "CollateralEstimationFees",
                column: "PCEId",
                principalTable: "ProductionCapacityEstimations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileUploads_ProductionCapacityEstimations_PCEId",
                table: "FileUploads",
                column: "PCEId",
                principalTable: "ProductionCapacityEstimations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimePeriod_ProductionCapacityEstimations_ProductionCapacityEstimationId",
                table: "TimePeriod",
                column: "ProductionCapacityEstimationId",
                principalTable: "ProductionCapacityEstimations",
                principalColumn: "Id");
        }
    }
}
