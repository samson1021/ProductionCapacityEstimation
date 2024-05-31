using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class first : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreateRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreateRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    emp_ID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    locationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pr_Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    supervisorEmpId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    supervisorID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    supervisor_name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rejects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CollateralId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RejectedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RejectionComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rejects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UploadFiles",
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
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CollateralId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreateUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    emp_ID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Branch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DistrictId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupervisorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreateUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreateUsers_CreateRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "CreateRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreateUsers_CreateUsers_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CreateUsers_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Signatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Emp_Id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignatureBase64String = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignatureFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Signatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Signatures_UploadFiles_SignatureFileId",
                        column: x => x.SignatureFileId,
                        principalTable: "UploadFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CaseNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DistrictId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BussinessLicenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Segement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaseOriginatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CaseOriginatordId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cases_CreateUsers_CaseOriginatordId",
                        column: x => x.CaseOriginatordId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Cases_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cases_UploadFiles_BussinessLicenceId",
                        column: x => x.BussinessLicenceId,
                        principalTable: "UploadFiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ConsecutiveNumbers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NextNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsecutiveNumbers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsecutiveNumbers_CreateUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Corrections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CollateralID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentedAttribute = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentedByUserIdsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Corrections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Corrections_CreateUsers_CommentedByUserIdsId",
                        column: x => x.CommentedByUserIdsId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CaseComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseComments_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaseComments_CreateUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CaseSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseSchedules_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaseSchedules_CreateUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                       onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CaseTerminates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseTerminates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseTerminates_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaseTerminates_CreateUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CaseTimeLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentStage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseTimeLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseTimeLines_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaseTimeLines_CreateUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Collaterals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyOwner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    PlateNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChassisNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EngineMotorNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManufactureYear = table.Column<int>(type: "int", nullable: true),
                    InvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CollateralType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Zone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Wereda = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kebele = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HouseNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModelNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentStage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FloorNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BuildingStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HouseArea = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ownership = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeOfFarming = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurposeOfTheLand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlotOfLand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LHCNo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collaterals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Collaterals_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Collaterals_CreateUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CaseAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CollateralId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseAssignments_Collaterals_CollateralId",
                        column: x => x.CollateralId,
                        principalTable: "Collaterals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CaseAssignments_CreateUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CollateralReestimations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CollateralId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollateralReestimations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollateralReestimations_Collaterals_CollateralId",
                        column: x => x.CollateralId,
                        principalTable: "Collaterals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConstMngAgrMachineries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CollateralId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvaluatorUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckerUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    constructionMiningAgriculturalMachineryType = table.Column<int>(type: "int", nullable: false),
                    EngineType = table.Column<int>(type: "int", nullable: false),
                    PowerSupply = table.Column<int>(type: "int", nullable: false),
                    NoOfCylinder = table.Column<int>(type: "int", nullable: false),
                    TransmissionType = table.Column<int>(type: "int", nullable: false),
                    IgnitionSystem = table.Column<int>(type: "int", nullable: false),
                    CoolingType = table.Column<int>(type: "int", nullable: false),
                    EnginePower = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkingProductionCapacity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechnologyStandard = table.Column<int>(type: "int", nullable: false),
                    MakerPreferenceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CabinType = table.Column<int>(type: "int", nullable: false),
                    NumberOfAxle = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MakerCompany = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentEqpmntCondition = table.Column<int>(type: "int", nullable: false),
                    AllocatedPointsRange = table.Column<int>(type: "int", nullable: false),
                    ModelNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EngineNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChassisNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerialNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlateNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TDNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearOfManufacture = table.Column<int>(type: "int", nullable: false),
                    CountryOfOrigin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MarketShareFactor = table.Column<double>(type: "float", nullable: false),
                    DepreciationRate = table.Column<double>(type: "float", nullable: false),
                    EqpmntConditionFactor = table.Column<double>(type: "float", nullable: false),
                    ReplacementCost = table.Column<double>(type: "float", nullable: false),
                    NetEstimationValue = table.Column<double>(type: "float", nullable: false),
                    PhysicalAndInstallationAssesment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OverallSurveyAssesment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceValue = table.Column<double>(type: "float", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExchangeRate = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConstMngAgrMachineries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConstMngAgrMachineries_Collaterals_CollateralId",
                        column: x => x.CollateralId,
                        principalTable: "Collaterals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConstMngAgrMachineries_CreateUsers_CheckerUserID",
                        column: x => x.CheckerUserID,
                        principalTable: "CreateUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ConstMngAgrMachineries_CreateUsers_EvaluatorUserID",
                        column: x => x.EvaluatorUserID,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IndBldgFacilityEquipment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CollateralId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvaluatorUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckerUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IndustrialBuildingMachineryType = table.Column<int>(type: "int", nullable: false),
                    EngineType = table.Column<int>(type: "int", nullable: false),
                    PowerSupply = table.Column<int>(type: "int", nullable: false),
                    MotorPower = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkingProductionCapacity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OtherTechSpec = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MakerCompany = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechnologyStandard = table.Column<int>(type: "int", nullable: false),
                    MakerPreferenceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentEqpmntCondition = table.Column<int>(type: "int", nullable: false),
                    AllocatedPointsRange = table.Column<int>(type: "int", nullable: false),
                    ModelNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerialNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EngineNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearOfManufacture = table.Column<int>(type: "int", nullable: false),
                    CountryOfOrigin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MarketShareFactor = table.Column<double>(type: "float", nullable: false),
                    DepreciationRate = table.Column<double>(type: "float", nullable: false),
                    EqpmntConditionFactor = table.Column<double>(type: "float", nullable: false),
                    ReplacementCost = table.Column<double>(type: "float", nullable: false),
                    NetEstimationValue = table.Column<double>(type: "float", nullable: false),
                    PhysicalAndInstallationAssesment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OverallSurveyAssesment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceValue = table.Column<double>(type: "float", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExchangeRate = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndBldgFacilityEquipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndBldgFacilityEquipment_Collaterals_CollateralId",
                        column: x => x.CollateralId,
                        principalTable: "Collaterals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndBldgFacilityEquipment_CreateUsers_CheckerUserID",
                        column: x => x.CheckerUserID,
                        principalTable: "CreateUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IndBldgFacilityEquipment_CreateUsers_EvaluatorUserID",
                        column: x => x.EvaluatorUserID,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MotorVehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CollateralId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvaluatorUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckerUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EngineType = table.Column<int>(type: "int", nullable: false),
                    NoOfCylinder = table.Column<int>(type: "int", nullable: false),
                    TransmissionType = table.Column<int>(type: "int", nullable: false),
                    coolingSystem = table.Column<int>(type: "int", nullable: false),
                    EnginePower = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoadingCapacity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BodyType = table.Column<int>(type: "int", nullable: false),
                    CabinType = table.Column<int>(type: "int", nullable: false),
                    NumberOfAxle = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotorVehicleMake = table.Column<int>(type: "int", nullable: false),
                    CurrentEqpmntCondition = table.Column<int>(type: "int", nullable: false),
                    AllocatedPointsRange = table.Column<int>(type: "int", nullable: false),
                    ModelNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EngineNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChassisNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerialNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlateNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TDNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearOfManufacture = table.Column<int>(type: "int", nullable: false),
                    CountryOfOrigin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MarketShareFactor = table.Column<double>(type: "float", nullable: false),
                    DepreciationRate = table.Column<double>(type: "float", nullable: false),
                    EqpmntConditionFactor = table.Column<double>(type: "float", nullable: false),
                    ReplacementCost = table.Column<double>(type: "float", nullable: false),
                    NetEstimationValue = table.Column<double>(type: "float", nullable: false),
                    PhysicalAndInstallationAssesment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OverallSurveyAssesment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceValue = table.Column<double>(type: "float", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExchangeRate = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotorVehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MotorVehicles_Collaterals_CollateralId",
                        column: x => x.CollateralId,
                        principalTable: "Collaterals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MotorVehicles_CreateUsers_CheckerUserID",
                        column: x => x.CheckerUserID,
                        principalTable: "CreateUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MotorVehicles_CreateUsers_EvaluatorUserID",
                        column: x => x.EvaluatorUserID,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaseAssignments_CollateralId",
                table: "CaseAssignments",
                column: "CollateralId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseAssignments_UserId",
                table: "CaseAssignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseComments_AuthorId",
                table: "CaseComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseComments_CaseId",
                table: "CaseComments",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Cases_BussinessLicenceId",
                table: "Cases",
                column: "BussinessLicenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Cases_CaseOriginatordId",
                table: "Cases",
                column: "CaseOriginatordId");

            migrationBuilder.CreateIndex(
                name: "IX_Cases_DistrictId",
                table: "Cases",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseSchedules_CaseId",
                table: "CaseSchedules",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseSchedules_UserId",
                table: "CaseSchedules",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseTerminates_CaseId",
                table: "CaseTerminates",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseTerminates_UserId",
                table: "CaseTerminates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseTimeLines_CaseId",
                table: "CaseTimeLines",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseTimeLines_UserId",
                table: "CaseTimeLines",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CollateralReestimations_CollateralId",
                table: "CollateralReestimations",
                column: "CollateralId");

            migrationBuilder.CreateIndex(
                name: "IX_Collaterals_CaseId",
                table: "Collaterals",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Collaterals_CreatedById",
                table: "Collaterals",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ConsecutiveNumbers_UserId",
                table: "ConsecutiveNumbers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ConstMngAgrMachineries_CheckerUserID",
                table: "ConstMngAgrMachineries",
                column: "CheckerUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ConstMngAgrMachineries_CollateralId",
                table: "ConstMngAgrMachineries",
                column: "CollateralId");

            migrationBuilder.CreateIndex(
                name: "IX_ConstMngAgrMachineries_EvaluatorUserID",
                table: "ConstMngAgrMachineries",
                column: "EvaluatorUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Corrections_CommentedByUserIdsId",
                table: "Corrections",
                column: "CommentedByUserIdsId");

            migrationBuilder.CreateIndex(
                name: "IX_CreateUsers_DistrictId",
                table: "CreateUsers",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_CreateUsers_RoleId",
                table: "CreateUsers",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_CreateUsers_SupervisorId",
                table: "CreateUsers",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_IndBldgFacilityEquipment_CheckerUserID",
                table: "IndBldgFacilityEquipment",
                column: "CheckerUserID");

            migrationBuilder.CreateIndex(
                name: "IX_IndBldgFacilityEquipment_CollateralId",
                table: "IndBldgFacilityEquipment",
                column: "CollateralId");

            migrationBuilder.CreateIndex(
                name: "IX_IndBldgFacilityEquipment_EvaluatorUserID",
                table: "IndBldgFacilityEquipment",
                column: "EvaluatorUserID");

            migrationBuilder.CreateIndex(
                name: "IX_MotorVehicles_CheckerUserID",
                table: "MotorVehicles",
                column: "CheckerUserID");

            migrationBuilder.CreateIndex(
                name: "IX_MotorVehicles_CollateralId",
                table: "MotorVehicles",
                column: "CollateralId");

            migrationBuilder.CreateIndex(
                name: "IX_MotorVehicles_EvaluatorUserID",
                table: "MotorVehicles",
                column: "EvaluatorUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Signatures_SignatureFileId",
                table: "Signatures",
                column: "SignatureFileId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseAssignments");

            migrationBuilder.DropTable(
                name: "CaseComments");

            migrationBuilder.DropTable(
                name: "CaseSchedules");

            migrationBuilder.DropTable(
                name: "CaseTerminates");

            migrationBuilder.DropTable(
                name: "CaseTimeLines");

            migrationBuilder.DropTable(
                name: "CollateralReestimations");

            migrationBuilder.DropTable(
                name: "ConsecutiveNumbers");

            migrationBuilder.DropTable(
                name: "ConstMngAgrMachineries");

            migrationBuilder.DropTable(
                name: "Corrections");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "IndBldgFacilityEquipment");

            migrationBuilder.DropTable(
                name: "MotorVehicles");

            migrationBuilder.DropTable(
                name: "Rejects");

            migrationBuilder.DropTable(
                name: "Signatures");

            migrationBuilder.DropTable(
                name: "Collaterals");

            migrationBuilder.DropTable(
                name: "Cases");

            migrationBuilder.DropTable(
                name: "CreateUsers");

            migrationBuilder.DropTable(
                name: "UploadFiles");

            migrationBuilder.DropTable(
                name: "CreateRoles");

            migrationBuilder.DropTable(
                name: "Districts");
        }
    }
}
