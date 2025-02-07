using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class dbdata : Migration
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
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupervisorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    company = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    CreationAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cases_CreateUsers_CaseOriginatorId",
                        column: x => x.CaseOriginatorId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "PCECases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CaseNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Segment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DistrictId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessLicenseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PCECaseOriginatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCECases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PCECases_CreateUsers_PCECaseOriginatorId",
                        column: x => x.PCECaseOriginatorId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PCECases_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PCECases_UploadFiles_BusinessLicenseId",
                        column: x => x.BusinessLicenseId,
                        principalTable: "UploadFiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReturnedProductions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PCEId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReturnedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReturnedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnedProductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReturnedProductions_CreateUsers_ReturnedById",
                        column: x => x.ReturnedById,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Signatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                        name: "FK_Signatures_CreateUsers_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Signatures_UploadFiles_SignatureFileId",
                        column: x => x.SignatureFileId,
                        principalTable: "UploadFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskManagments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaseOrginatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SharingReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaskStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PriorityType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskManagments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskManagments_CreateUsers_AssignedId",
                        column: x => x.AssignedId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskManagments_CreateUsers_CaseOrginatorId",
                        column: x => x.CaseOrginatorId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskNotifications_CreateUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "PCECaseComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PCECaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCECaseComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PCECaseComments_CreateUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PCECaseComments_PCECases_PCECaseId",
                        column: x => x.PCECaseId,
                        principalTable: "PCECases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PCECaseSchedules",
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
                    table.PrimaryKey("PK_PCECaseSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PCECaseSchedules_CreateUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PCECaseSchedules_PCECases_PCECaseId",
                        column: x => x.PCECaseId,
                        principalTable: "PCECases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PCECaseTerminates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TerminatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PCECaseOriginatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PCECaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCECaseTerminates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PCECaseTerminates_CreateUsers_PCECaseOriginatorId",
                        column: x => x.PCECaseOriginatorId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PCECaseTerminates_PCECases_PCECaseId",
                        column: x => x.PCECaseId,
                        principalTable: "PCECases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PCECaseTimeLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PCECaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentStage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCECaseTimeLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PCECaseTimeLines_CreateUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PCECaseTimeLines_PCECases_PCECaseId",
                        column: x => x.PCECaseId,
                        principalTable: "PCECases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductionCapacities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PCECaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyOwner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryOfOrgin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BusinessLicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModelNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManufactureYear = table.Column<int>(type: "int", nullable: true),
                    InvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MachineryInstalledPlace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LHCNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Industrialpark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Zone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Wereda = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kebele = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HouseNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentStage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedEvaluatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionCapacities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionCapacities_CreateUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductionCapacities_PCECases_PCECaseId",
                        column: x => x.PCECaseId,
                        principalTable: "PCECases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CommentById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskComments_CreateUsers_CommentById",
                        column: x => x.CommentById,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskComments_TaskManagments_TaskId",
                        column: x => x.TaskId,
                        principalTable: "TaskManagments",
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
                    ScaleOfOperation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoOfProductionLine = table.Column<string>(type: "nvarchar(max)", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "PCECaseAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ProductionCapacityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCECaseAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PCECaseAssignments_CreateUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PCECaseAssignments_ProductionCapacities_ProductionCapacityId",
                        column: x => x.ProductionCapacityId,
                        principalTable: "ProductionCapacities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PCEEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PCEId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvaluatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductionLineOrEquipmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutputType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutputPhase = table.Column<int>(type: "int", nullable: false),
                    ShiftsPerDay = table.Column<int>(type: "int", nullable: true),
                    WorkingDaysPerMonth = table.Column<int>(type: "int", nullable: true),
                    EffectiveProductionHourType = table.Column<int>(type: "int", nullable: true),
                    EffectiveProductionHour = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductionUnit = table.Column<int>(type: "int", nullable: false),
                    ProductionMeasurement = table.Column<int>(type: "int", nullable: false),
                    EstimatedProductionCapacity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BottleneckProductionLineCapacity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverallActualCurrentCapacity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechnicalObsolescenceStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepreciationRateApplied = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discrepancies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActualProductionCapacity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DesignProductionCapacity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttainableProductionCapacity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FactorsAffectingProductionCapacity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineFunctionalityStatus = table.Column<int>(type: "int", nullable: false),
                    MachineNonFunctionalityReason = table.Column<int>(type: "int", nullable: true),
                    OtherMachineNonFunctionalityReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InspectionPlace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SurveyRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCEEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PCEEvaluations_CreateUsers_EvaluatorId",
                        column: x => x.EvaluatorId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PCEEvaluations_ProductionCapacities_PCEId",
                        column: x => x.PCEId,
                        principalTable: "ProductionCapacities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductionReestimations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ProductionCapacityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionReestimations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionReestimations_ProductionCapacities_ProductionCapacityId",
                        column: x => x.ProductionCapacityId,
                        principalTable: "ProductionCapacities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DateTimeRanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PCEEId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DateTimeRanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DateTimeRanges_PCEEvaluations_PCEEId",
                        column: x => x.PCEEId,
                        principalTable: "PCEEvaluations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimeIntervals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PCEEId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Start = table.Column<TimeSpan>(type: "time", nullable: false),
                    End = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeIntervals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeIntervals_PCEEvaluations_PCEEId",
                        column: x => x.PCEEId,
                        principalTable: "PCEEvaluations",
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
                name: "IX_Cases_CaseOriginatorId",
                table: "Cases",
                column: "CaseOriginatorId");

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
                name: "IX_DateTimeRanges_PCEEId",
                table: "DateTimeRanges",
                column: "PCEEId",
                unique: true);

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
                name: "IX_PCECaseAssignments_ProductionCapacityId",
                table: "PCECaseAssignments",
                column: "ProductionCapacityId");

            migrationBuilder.CreateIndex(
                name: "IX_PCECaseAssignments_UserId",
                table: "PCECaseAssignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PCECaseComments_AuthorId",
                table: "PCECaseComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_PCECaseComments_PCECaseId",
                table: "PCECaseComments",
                column: "PCECaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PCECases_BusinessLicenseId",
                table: "PCECases",
                column: "BusinessLicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_PCECases_DistrictId",
                table: "PCECases",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_PCECases_PCECaseOriginatorId",
                table: "PCECases",
                column: "PCECaseOriginatorId");

            migrationBuilder.CreateIndex(
                name: "IX_PCECaseSchedules_PCECaseId",
                table: "PCECaseSchedules",
                column: "PCECaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PCECaseSchedules_UserId",
                table: "PCECaseSchedules",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PCECaseTerminates_PCECaseId",
                table: "PCECaseTerminates",
                column: "PCECaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PCECaseTerminates_PCECaseOriginatorId",
                table: "PCECaseTerminates",
                column: "PCECaseOriginatorId");

            migrationBuilder.CreateIndex(
                name: "IX_PCECaseTimeLines_PCECaseId",
                table: "PCECaseTimeLines",
                column: "PCECaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PCECaseTimeLines_UserId",
                table: "PCECaseTimeLines",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PCEEvaluations_EvaluatorId",
                table: "PCEEvaluations",
                column: "EvaluatorId");

            migrationBuilder.CreateIndex(
                name: "IX_PCEEvaluations_PCEId",
                table: "PCEEvaluations",
                column: "PCEId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacities_CreatedById",
                table: "ProductionCapacities",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacities_PCECaseId",
                table: "ProductionCapacities",
                column: "PCECaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionReestimations_ProductionCapacityId",
                table: "ProductionReestimations",
                column: "ProductionCapacityId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnedProductions_ReturnedById",
                table: "ReturnedProductions",
                column: "ReturnedById");

            migrationBuilder.CreateIndex(
                name: "IX_Signatures_CreateUserId",
                table: "Signatures",
                column: "CreateUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Signatures_SignatureFileId",
                table: "Signatures",
                column: "SignatureFileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskComments_CommentById",
                table: "TaskComments",
                column: "CommentById");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComments_TaskId",
                table: "TaskComments",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskManagments_AssignedId",
                table: "TaskManagments",
                column: "AssignedId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskManagments_CaseOrginatorId",
                table: "TaskManagments",
                column: "CaseOrginatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskNotifications_UserId",
                table: "TaskNotifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeIntervals_PCEEId",
                table: "TimeIntervals",
                column: "PCEEId");
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
                name: "DateTimeRanges");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "IndBldgFacilityEquipment");

            migrationBuilder.DropTable(
                name: "MotorVehicles");

            migrationBuilder.DropTable(
                name: "PCECaseAssignments");

            migrationBuilder.DropTable(
                name: "PCECaseComments");

            migrationBuilder.DropTable(
                name: "PCECaseSchedules");

            migrationBuilder.DropTable(
                name: "PCECaseTerminates");

            migrationBuilder.DropTable(
                name: "PCECaseTimeLines");

            migrationBuilder.DropTable(
                name: "ProductionReestimations");

            migrationBuilder.DropTable(
                name: "Rejects");

            migrationBuilder.DropTable(
                name: "ReturnedProductions");

            migrationBuilder.DropTable(
                name: "Signatures");

            migrationBuilder.DropTable(
                name: "TaskComments");

            migrationBuilder.DropTable(
                name: "TaskNotifications");

            migrationBuilder.DropTable(
                name: "TimeIntervals");

            migrationBuilder.DropTable(
                name: "Collaterals");

            migrationBuilder.DropTable(
                name: "TaskManagments");

            migrationBuilder.DropTable(
                name: "PCEEvaluations");

            migrationBuilder.DropTable(
                name: "Cases");

            migrationBuilder.DropTable(
                name: "ProductionCapacities");

            migrationBuilder.DropTable(
                name: "PCECases");

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
