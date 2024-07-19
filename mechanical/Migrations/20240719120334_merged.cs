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
                name: "FK_ProductionCaseAssignments_ProductionCapacities_ProductionCapacityId",
                table: "ProductionCaseAssignments");

            migrationBuilder.DropTable(
                name: "PlantCapacityEstimations");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductionCapacityId",
                table: "ProductionCaseAssignments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletionDate",
                table: "ProductionCaseAssignments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionCaseAssignments_ProductionCapacities_ProductionCapacityId",
                table: "ProductionCaseAssignments",
                column: "ProductionCapacityId",
                principalTable: "ProductionCapacities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionCaseAssignments_ProductionCapacities_ProductionCapacityId",
                table: "ProductionCaseAssignments");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductionCapacityId",
                table: "ProductionCaseAssignments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletionDate",
                table: "ProductionCaseAssignments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PlantCapacityEstimations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CollateralType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentStage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfInspection = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HouseNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kebele = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LHCNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NamePlant = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObsolescenceStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerNameLHC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerOfPlant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlantCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlantDepreciationRate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlantKebele = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlantRegion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlantSubCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlantWereda = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlantZone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurposeOfPCE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TradeLicenseNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Wereda = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YearOfManifacturing = table.Column<int>(type: "int", nullable: false),
                    Zone = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlantCapacityEstimations_CaseId",
                table: "PlantCapacityEstimations",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantCapacityEstimations_CreatedById",
                table: "PlantCapacityEstimations",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionCaseAssignments_ProductionCapacities_ProductionCapacityId",
                table: "ProductionCaseAssignments",
                column: "ProductionCapacityId",
                principalTable: "ProductionCapacities",
                principalColumn: "Id");
        }
    }
}
