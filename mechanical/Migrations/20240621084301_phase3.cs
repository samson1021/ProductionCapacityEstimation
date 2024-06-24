using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class phase3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CollateralEstimationFees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CollateralClass = table.Column<int>(type: "int", nullable: false),
                    CollateralCategory = table.Column<int>(type: "int", nullable: false),
                    UnitOfMeasure = table.Column<int>(type: "int", nullable: false),
                    EstimationFeePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollateralEstimationFees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollateralEstimationFees_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DatePeriod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatePeriod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DateTimePeriod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DateTimePeriod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionCapacityEstimations",
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
                    table.PrimaryKey("PK_ProductionCapacityEstimations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionCapacityEstimations_Cases_PCECaseId",
                        column: x => x.PCECaseId,
                        principalTable: "Cases",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductionCapacityEstimations_DateTimePeriod_TimeConsumedToCheckId",
                        column: x => x.TimeConsumedToCheckId,
                        principalTable: "DateTimePeriod",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FileUploads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PCECaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UploadAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductionCapacityEstimationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductionCapacityEstimationId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileUploads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileUploads_ProductionCapacityEstimations_ProductionCapacityEstimationId",
                        column: x => x.ProductionCapacityEstimationId,
                        principalTable: "ProductionCapacityEstimations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileUploads_ProductionCapacityEstimations_ProductionCapacityEstimationId1",
                        column: x => x.ProductionCapacityEstimationId1,
                        principalTable: "ProductionCapacityEstimations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductionCapacitySchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ProductionCapacityEstimationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionCapacitySchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionCapacitySchedules_ProductionCapacityEstimations_ProductionCapacityEstimationId",
                        column: x => x.ProductionCapacityEstimationId,
                        principalTable: "ProductionCapacityEstimations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimePeriod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Start = table.Column<TimeSpan>(type: "time", nullable: false),
                    End = table.Column<TimeSpan>(type: "time", nullable: false),
                    ProductionCapacityEstimationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimePeriod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimePeriod_ProductionCapacityEstimations_ProductionCapacityEstimationId",
                        column: x => x.ProductionCapacityEstimationId,
                        principalTable: "ProductionCapacityEstimations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollateralEstimationFees_CaseId",
                table: "CollateralEstimationFees",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUploads_ProductionCapacityEstimationId",
                table: "FileUploads",
                column: "ProductionCapacityEstimationId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUploads_ProductionCapacityEstimationId1",
                table: "FileUploads",
                column: "ProductionCapacityEstimationId1");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacityEstimations_PCECaseId",
                table: "ProductionCapacityEstimations",
                column: "PCECaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacityEstimations_TimeConsumedToCheckId",
                table: "ProductionCapacityEstimations",
                column: "TimeConsumedToCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacitySchedules_ProductionCapacityEstimationId",
                table: "ProductionCapacitySchedules",
                column: "ProductionCapacityEstimationId");

            migrationBuilder.CreateIndex(
                name: "IX_TimePeriod_ProductionCapacityEstimationId",
                table: "TimePeriod",
                column: "ProductionCapacityEstimationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollateralEstimationFees");

            migrationBuilder.DropTable(
                name: "DatePeriod");

            migrationBuilder.DropTable(
                name: "FileUploads");

            migrationBuilder.DropTable(
                name: "ProductionCapacitySchedules");

            migrationBuilder.DropTable(
                name: "TimePeriod");

            migrationBuilder.DropTable(
                name: "ProductionCapacityEstimations");

            migrationBuilder.DropTable(
                name: "DateTimePeriod");
        }
    }
}
