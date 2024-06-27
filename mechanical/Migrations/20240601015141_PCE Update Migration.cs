using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class PCEUpdateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PCECaseTimeLines");

            migrationBuilder.DropTable(
                name: "PCECases");
        }
    }
}
