using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class merged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollateralEstimationFees_Cases_CaseId",
                table: "CollateralEstimationFees");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadFiles_ProductionCapacityEstimations_ProductionCapacityEstimationId",
                table: "UploadFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadFiles_ProductionCapacityEstimations_ProductionCapacityEstimationId1",
                table: "UploadFiles");

            migrationBuilder.DropTable(
                name: "ProductionCapacitySchedule");

            migrationBuilder.DropTable(
                name: "ShiftHour");

            migrationBuilder.DropTable(
                name: "ProductionCapacityEstimations");

            migrationBuilder.DropIndex(
                name: "IX_UploadFiles_ProductionCapacityEstimationId",
                table: "UploadFiles");

            migrationBuilder.DropIndex(
                name: "IX_CollateralEstimationFees_CaseId",
                table: "CollateralEstimationFees");

            migrationBuilder.DropColumn(
                name: "ProductionCapacityEstimationId",
                table: "UploadFiles");

            migrationBuilder.DropColumn(
                name: "CaseId",
                table: "CollateralEstimationFees");

            migrationBuilder.RenameColumn(
                name: "ProductionCapacityEstimationId1",
                table: "UploadFiles",
                newName: "PCEEvaluationId");

            migrationBuilder.RenameIndex(
                name: "IX_UploadFiles_ProductionCapacityEstimationId1",
                table: "UploadFiles",
                newName: "IX_UploadFiles_PCEEvaluationId");

            migrationBuilder.AddColumn<Guid>(
                name: "PCEEId",
                table: "CollateralEstimationFees",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "CollateralEstimationFees",
                type: "nvarchar(max)",
                nullable: true);

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
                name: "ProductionCapcityCorrections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PCECaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductionCapacityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentedAttribute = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentedByUserIdsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionCapcityCorrections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionCapcityCorrections_CreateUsers_CommentedByUserIdsId",
                        column: x => x.CommentedByUserIdsId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductionRejects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PCECaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RejectedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RejectionComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionRejects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Returns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PCEId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReturnedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Returns", x => x.Id);
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
                name: "PlantCapacityEstimations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CollateralType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Zone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Wereda = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kebele = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HouseNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerOfPlant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TradeLicenseNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LHCNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerNameLHC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NamePlant = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearOfManifacturing = table.Column<int>(type: "int", nullable: false),
                    PurposeOfPCE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObsolescenceStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlantDepreciationRate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlantRegion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlantCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlantZone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlantSubCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlantWereda = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlantKebele = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfInspection = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentStage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantCapacityEstimations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlantCapacityEstimations_CreateUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlantCapacityEstimations_PCECases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "PCECases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ProductionCapacities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PCECaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyOwner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModelNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductionBussinessLicence = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManufactureYear = table.Column<int>(type: "int", nullable: true),
                    InvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MachineryInstalledPlace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LHCNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Industrialpark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Zone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Wereda = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kebele = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HouseNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlantName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ObsolescenceStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlantDepreciationRate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfInspection = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentStage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvaluatorUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CheckerUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionCapacities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionCapacities_CreateUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "CreateUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductionCapacities_PCECases_PCECaseId",
                        column: x => x.PCECaseId,
                        principalTable: "PCECases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionCaseSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PCECaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionCaseSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionCaseSchedules_CreateUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductionCaseSchedules_PCECases_PCECaseId",
                        column: x => x.PCECaseId,
                        principalTable: "PCECases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "PCEUploadFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Catagory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PCECaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlantCapacityEstimationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCEUploadFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PCEUploadFiles_CreateUsers_userId",
                        column: x => x.userId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PCEUploadFiles_PCECases_PCECaseId",
                        column: x => x.PCECaseId,
                        principalTable: "PCECases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_PCEUploadFiles_PlantCapacityEstimations_PlantCapacityEstimationId",
                        column: x => x.PlantCapacityEstimationId,
                        principalTable: "PlantCapacityEstimations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "PCEEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PCEId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvaluatorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductionLineOrEquipmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutputPhase = table.Column<int>(type: "int", nullable: true),
                    OriginCountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductionUnit = table.Column<int>(type: "int", nullable: true),
                    WorkingDaysPerMonth = table.Column<int>(type: "int", nullable: true),
                    ShiftsPerDay = table.Column<int>(type: "int", nullable: true),
                    ProductionMeasurement = table.Column<int>(type: "int", nullable: true),
                    EstimatedProductionCapacity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BottleneckProductionLineCapacity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverallActualCurrentPlantCapacity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeConsumedToCheckId = table.Column<int>(type: "int", nullable: true),
                    TechnicalObsolescenceStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepreciationRateApplied = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Discrepancies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EffectiveProductionHourType = table.Column<int>(type: "int", nullable: true),
                    EffectiveProductionHour = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DesignProductionCapacity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttainableProductionCapacity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActualProductionCapacity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FactorsAffectingProductionCapacity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MachineFunctionalityStatus = table.Column<int>(type: "int", nullable: true),
                    MachineNonFunctionalityReason = table.Column<int>(type: "int", nullable: true),
                    OtherMachineNonFunctionalityReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InspectionPlace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SurveyRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCEEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PCEEvaluations_CreateUsers_EvaluatorID",
                        column: x => x.EvaluatorID,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PCEEvaluations_DateTimePeriod_TimeConsumedToCheckId",
                        column: x => x.TimeConsumedToCheckId,
                        principalTable: "DateTimePeriod",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PCEEvaluations_ProductionCapacities_PCEId",
                        column: x => x.PCEId,
                        principalTable: "ProductionCapacities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ProductionCapacityReestimations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ProductionCapacityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionCapacityReestimations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionCapacityReestimations_ProductionCapacities_ProductionCapacityId",
                        column: x => x.ProductionCapacityId,
                        principalTable: "ProductionCapacities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionCaseAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PCECaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductionCapacityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionCaseAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionCaseAssignments_CreateUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductionCaseAssignments_ProductionCapacities_ProductionCapacityId",
                        column: x => x.ProductionCapacityId,
                        principalTable: "ProductionCapacities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductionReestimations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PCECaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductionCapacityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionReestimations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionReestimations_ProductionCapacities_ProductionCapacityId",
                        column: x => x.ProductionCapacityId,
                        principalTable: "ProductionCapacities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TimePeriod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Start = table.Column<TimeSpan>(type: "time", nullable: false),
                    End = table.Column<TimeSpan>(type: "time", nullable: false),
                    PCEEvaluationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimePeriod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimePeriod_PCEEvaluations_PCEEvaluationId",
                        column: x => x.PCEEvaluationId,
                        principalTable: "PCEEvaluations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollateralEstimationFees_PCEEId",
                table: "CollateralEstimationFees",
                column: "PCEEId");

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
                name: "IX_PCEEvaluations_EvaluatorID",
                table: "PCEEvaluations",
                column: "EvaluatorID");

            migrationBuilder.CreateIndex(
                name: "IX_PCEEvaluations_PCEId",
                table: "PCEEvaluations",
                column: "PCEId");

            migrationBuilder.CreateIndex(
                name: "IX_PCEEvaluations_TimeConsumedToCheckId",
                table: "PCEEvaluations",
                column: "TimeConsumedToCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_PCESchedules_PCECaseId",
                table: "PCESchedules",
                column: "PCECaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PCEUploadFiles_PCECaseId",
                table: "PCEUploadFiles",
                column: "PCECaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PCEUploadFiles_PlantCapacityEstimationId",
                table: "PCEUploadFiles",
                column: "PlantCapacityEstimationId");

            migrationBuilder.CreateIndex(
                name: "IX_PCEUploadFiles_userId",
                table: "PCEUploadFiles",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantCapacityEstimations_CaseId",
                table: "PlantCapacityEstimations",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantCapacityEstimations_CreatedById",
                table: "PlantCapacityEstimations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacities_CreatedById",
                table: "ProductionCapacities",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacities_PCECaseId",
                table: "ProductionCapacities",
                column: "PCECaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacityReestimations_ProductionCapacityId",
                table: "ProductionCapacityReestimations",
                column: "ProductionCapacityId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapcityCorrections_CommentedByUserIdsId",
                table: "ProductionCapcityCorrections",
                column: "CommentedByUserIdsId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCaseAssignments_ProductionCapacityId",
                table: "ProductionCaseAssignments",
                column: "ProductionCapacityId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCaseAssignments_UserId",
                table: "ProductionCaseAssignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCaseSchedules_PCECaseId",
                table: "ProductionCaseSchedules",
                column: "PCECaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCaseSchedules_UserId",
                table: "ProductionCaseSchedules",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionReestimations_ProductionCapacityId",
                table: "ProductionReestimations",
                column: "ProductionCapacityId");

            migrationBuilder.CreateIndex(
                name: "IX_TimePeriod_PCEEvaluationId",
                table: "TimePeriod",
                column: "PCEEvaluationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CollateralEstimationFees_PCEEvaluations_PCEEId",
                table: "CollateralEstimationFees",
                column: "PCEEId",
                principalTable: "PCEEvaluations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadFiles_PCEEvaluations_PCEEvaluationId",
                table: "UploadFiles",
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
                name: "FK_UploadFiles_PCEEvaluations_PCEEvaluationId",
                table: "UploadFiles");

            migrationBuilder.DropTable(
                name: "DatePeriod");

            migrationBuilder.DropTable(
                name: "PCECaseTimeLines");

            migrationBuilder.DropTable(
                name: "PCESchedules");

            migrationBuilder.DropTable(
                name: "PCEUploadFiles");

            migrationBuilder.DropTable(
                name: "ProductionCapacityReestimations");

            migrationBuilder.DropTable(
                name: "ProductionCapcityCorrections");

            migrationBuilder.DropTable(
                name: "ProductionCaseAssignments");

            migrationBuilder.DropTable(
                name: "ProductionCaseSchedules");

            migrationBuilder.DropTable(
                name: "ProductionReestimations");

            migrationBuilder.DropTable(
                name: "ProductionRejects");

            migrationBuilder.DropTable(
                name: "Returns");

            migrationBuilder.DropTable(
                name: "TimePeriod");

            migrationBuilder.DropTable(
                name: "PlantCapacityEstimations");

            migrationBuilder.DropTable(
                name: "PCEEvaluations");

            migrationBuilder.DropTable(
                name: "DateTimePeriod");

            migrationBuilder.DropTable(
                name: "ProductionCapacities");

            migrationBuilder.DropTable(
                name: "PCECases");

            migrationBuilder.DropIndex(
                name: "IX_CollateralEstimationFees_PCEEId",
                table: "CollateralEstimationFees");

            migrationBuilder.DropColumn(
                name: "PCEEId",
                table: "CollateralEstimationFees");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "CollateralEstimationFees");

            migrationBuilder.RenameColumn(
                name: "PCEEvaluationId",
                table: "UploadFiles",
                newName: "ProductionCapacityEstimationId1");

            migrationBuilder.RenameIndex(
                name: "IX_UploadFiles_PCEEvaluationId",
                table: "UploadFiles",
                newName: "IX_UploadFiles_ProductionCapacityEstimationId1");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductionCapacityEstimationId",
                table: "UploadFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CaseId",
                table: "CollateralEstimationFees",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ProductionCapacityEstimations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ActualProductionCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AttainableProductionCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BottleneckProductionLineCapacity = table.Column<int>(type: "int", nullable: true),
                    CountryOfOrigin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepreciationRateApplied = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DesignProductionCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Discrepancies = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EffectiveProductionHour = table.Column<int>(type: "int", nullable: true),
                    EffectiveProductionHourPerShift = table.Column<int>(type: "int", nullable: true),
                    EffectiveProductionHourType = table.Column<int>(type: "int", nullable: true),
                    EstimatedProductionCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FactorsAffectingProductionCapacity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MachineFunctionalityReason = table.Column<int>(type: "int", nullable: true),
                    MachineFunctionalityStatus = table.Column<int>(type: "int", nullable: true),
                    OtherMachineFunctionalityReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OverallActualCurrentPlantCapacity = table.Column<int>(type: "int", nullable: true),
                    PhaseOfOutput = table.Column<int>(type: "int", nullable: true),
                    PlaceOfInspection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductionLineOrEquipmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductionMeasurement = table.Column<int>(type: "int", nullable: true),
                    ProductionPerHour = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftsPerDay = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SurveyRemark = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechnicalObsolescenceStatus = table.Column<int>(type: "int", nullable: true),
                    TimeConsumedToCheckEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TimeConsumedToCheckStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TypeOfOutput = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitOfProduction = table.Column<int>(type: "int", nullable: true),
                    WorkingDaysPerMonth = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionCapacityEstimations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionCapacitySchedule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ProductionCapacityEstimationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionCapacitySchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionCapacitySchedule_ProductionCapacityEstimations_ProductionCapacityEstimationId",
                        column: x => x.ProductionCapacityEstimationId,
                        principalTable: "ProductionCapacityEstimations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftHour",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductionCapacityEstimationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "IX_CollateralEstimationFees_CaseId",
                table: "CollateralEstimationFees",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacitySchedule_ProductionCapacityEstimationId",
                table: "ProductionCapacitySchedule",
                column: "ProductionCapacityEstimationId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftHour_ProductionCapacityEstimationId",
                table: "ShiftHour",
                column: "ProductionCapacityEstimationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CollateralEstimationFees_Cases_CaseId",
                table: "CollateralEstimationFees",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
