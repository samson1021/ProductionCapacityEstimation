using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEntityAndDtoOfProductionCapacityEstimationToOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionCapacityEstimations_Cases_CaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.AlterColumn<int>(
                name: "UnitOfProduction",
                table: "ProductionCapacityEstimations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TechnicalObsolescenceStatus",
                table: "ProductionCapacityEstimations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProductionPerHour",
                table: "ProductionCapacityEstimations",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "ProductionMeasurement",
                table: "ProductionCapacityEstimations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PhaseOfOutput",
                table: "ProductionCapacityEstimations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MachineFunctionalityStatus",
                table: "ProductionCapacityEstimations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "EffectiveProductionHour",
                table: "ProductionCapacityEstimations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "DesignProductionCapacity",
                table: "ProductionCapacityEstimations",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<Guid>(
                name: "CaseId",
                table: "ProductionCapacityEstimations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<decimal>(
                name: "AttainableProductionCapacity",
                table: "ProductionCapacityEstimations",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ActualProductionCapacity",
                table: "ProductionCapacityEstimations",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "ProductionCapacityEstimations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "ProductionCapacityEstimations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ProductionCapacityEstimations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ProductionCapacitySchedule",
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
                    table.PrimaryKey("PK_ProductionCapacitySchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionCapacitySchedule_ProductionCapacityEstimations_ProductionCapacityEstimationId",
                        column: x => x.ProductionCapacityEstimationId,
                        principalTable: "ProductionCapacityEstimations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductionCapacitySchedule_ProductionCapacityEstimationId",
                table: "ProductionCapacitySchedule",
                column: "ProductionCapacityEstimationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionCapacityEstimations_Cases_CaseId",
                table: "ProductionCapacityEstimations",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionCapacityEstimations_Cases_CaseId",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropTable(
                name: "ProductionCapacitySchedule");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProductionCapacityEstimations");

            migrationBuilder.AlterColumn<int>(
                name: "UnitOfProduction",
                table: "ProductionCapacityEstimations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TechnicalObsolescenceStatus",
                table: "ProductionCapacityEstimations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ProductionPerHour",
                table: "ProductionCapacityEstimations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductionMeasurement",
                table: "ProductionCapacityEstimations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PhaseOfOutput",
                table: "ProductionCapacityEstimations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MachineFunctionalityStatus",
                table: "ProductionCapacityEstimations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EffectiveProductionHour",
                table: "ProductionCapacityEstimations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DesignProductionCapacity",
                table: "ProductionCapacityEstimations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CaseId",
                table: "ProductionCapacityEstimations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AttainableProductionCapacity",
                table: "ProductionCapacityEstimations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ActualProductionCapacity",
                table: "ProductionCapacityEstimations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionCapacityEstimations_Cases_CaseId",
                table: "ProductionCapacityEstimations",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
