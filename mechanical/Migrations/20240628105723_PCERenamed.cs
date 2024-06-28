using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class PCERenamed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreateUsers_CreateRole_RoleId",
                table: "CreateUsers");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_CreateRole_TempId1",
                table: "CreateRole");

            migrationBuilder.DropColumn(
                name: "TempId1",
                table: "CreateRole");

            migrationBuilder.RenameTable(
                name: "CreateRole",
                newName: "CreateRoles");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CreateRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CreateRoles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreateRoles",
                table: "CreateRoles",
                column: "Id");

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
                name: "PCECases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CaseNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DistrictId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentStage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MakerAssignmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RMUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCECases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PCECases_CreateUsers_RMUserId",
                        column: x => x.RMUserId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PCECases_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PCECaseTimeLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentStage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewCaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCECaseTimeLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PCECaseTimeLines_CreateUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PCECaseTimeLines_PCECases_NewCaseId",
                        column: x => x.NewCaseId,
                        principalTable: "PCECases",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PCESchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PCECaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_PCESchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PCESchedules_PCECases_PCECaseId",
                        column: x => x.PCECaseId,
                        principalTable: "PCECases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "CollateralEstimationFees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PCEId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                        name: "FK_CollateralEstimationFees_ProductionCapacityEstimations_PCEId",
                        column: x => x.PCEId,
                        principalTable: "ProductionCapacityEstimations",
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
                    PCEId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    UploadAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileUploads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileUploads_ProductionCapacityEstimations_PCEId",
                        column: x => x.PCEId,
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
                name: "IX_CollateralEstimationFees_PCEId",
                table: "CollateralEstimationFees",
                column: "PCEId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUploads_PCEId",
                table: "FileUploads",
                column: "PCEId");

            migrationBuilder.CreateIndex(
                name: "IX_PCECases_DistrictId",
                table: "PCECases",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_PCECases_RMUserId",
                table: "PCECases",
                column: "RMUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PCECaseTimeLines_NewCaseId",
                table: "PCECaseTimeLines",
                column: "NewCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PCECaseTimeLines_UserId",
                table: "PCECaseTimeLines",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PCESchedules_PCECaseId",
                table: "PCESchedules",
                column: "PCECaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacityEstimations_PCECaseId",
                table: "ProductionCapacityEstimations",
                column: "PCECaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacityEstimations_TimeConsumedToCheckId",
                table: "ProductionCapacityEstimations",
                column: "TimeConsumedToCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_TimePeriod_ProductionCapacityEstimationId",
                table: "TimePeriod",
                column: "ProductionCapacityEstimationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateUsers_CreateRoles_RoleId",
                table: "CreateUsers",
                column: "RoleId",
                principalTable: "CreateRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreateUsers_CreateRoles_RoleId",
                table: "CreateUsers");

            migrationBuilder.DropTable(
                name: "CollateralEstimationFees");

            migrationBuilder.DropTable(
                name: "DatePeriod");

            migrationBuilder.DropTable(
                name: "FileUploads");

            migrationBuilder.DropTable(
                name: "PCECaseTimeLines");

            migrationBuilder.DropTable(
                name: "PCESchedules");

            migrationBuilder.DropTable(
                name: "TimePeriod");

            migrationBuilder.DropTable(
                name: "ProductionCapacityEstimations");

            migrationBuilder.DropTable(
                name: "DateTimePeriod");

            migrationBuilder.DropTable(
                name: "PCECases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreateRoles",
                table: "CreateRoles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CreateRoles");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CreateRoles");

            migrationBuilder.RenameTable(
                name: "CreateRoles",
                newName: "CreateRole");

            migrationBuilder.AddColumn<Guid>(
                name: "TempId1",
                table: "CreateRole",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_CreateRole_TempId1",
                table: "CreateRole",
                column: "TempId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CreateUsers_CreateRole_RoleId",
                table: "CreateUsers",
                column: "RoleId",
                principalTable: "CreateRole",
                principalColumn: "TempId1",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
