using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class Manufacturing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModelNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductionBussinessLicence = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManufactureYear = table.Column<int>(type: "int", nullable: true),
                    InvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MachineryInstalledPlace = table.Column<int>(type: "int", nullable: false),
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
                    CurrentStage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvaluatorUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckerUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionCapacities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionCapacities_CreateUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductionCapacities_PCECases_PCECaseId",
                        column: x => x.PCECaseId,
                        principalTable: "PCECases",
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
                name: "ProductionCaseSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductionCaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PCECaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductionRejects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ProductionCapacityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RejectedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RejectionComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionRejects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionUploadFiles",
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
                    ProductionCapacityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionUploadFiles", x => x.Id);
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
                    ProductionCapacityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductionCapacityReestimations");

            migrationBuilder.DropTable(
                name: "ProductionCapcityCorrections");

            migrationBuilder.DropTable(
                name: "ProductionCaseAssignments");

            migrationBuilder.DropTable(
                name: "ProductionCaseSchedules");

            migrationBuilder.DropTable(
                name: "ProductionRejects");

            migrationBuilder.DropTable(
                name: "ProductionUploadFiles");

            migrationBuilder.DropTable(
                name: "ProductionCapacities");
        }
    }
}
