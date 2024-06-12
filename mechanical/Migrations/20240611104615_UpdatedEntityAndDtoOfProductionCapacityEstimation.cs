using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEntityAndDtoOfProductionCapacityEstimation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaseReferenceNumber",
                table: "ProductionCapacityEstimations");

            migrationBuilder.RenameColumn(
                name: "ReasonForNotFunctional",
                table: "ProductionCapacityEstimations",
                newName: "OtherMachineFunctionalityReason");

            migrationBuilder.AlterColumn<int>(
                name: "UnitOfProduction",
                table: "ProductionCapacityEstimations",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AttainableProductionCapacity",
                table: "ProductionCapacityEstimations",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "ActualProductionCapacity",
                table: "ProductionCapacityEstimations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "EffectiveProductionHourPerShift",
                table: "ProductionCapacityEstimations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MachineFunctionalityReason",
                table: "ProductionCapacityEstimations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ProductionPerHour",
                table: "ProductionCapacityEstimations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualProductionCapacity",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropColumn(
                name: "EffectiveProductionHourPerShift",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropColumn(
                name: "MachineFunctionalityReason",
                table: "ProductionCapacityEstimations");

            migrationBuilder.DropColumn(
                name: "ProductionPerHour",
                table: "ProductionCapacityEstimations");

            migrationBuilder.RenameColumn(
                name: "OtherMachineFunctionalityReason",
                table: "ProductionCapacityEstimations",
                newName: "ReasonForNotFunctional");

            migrationBuilder.AlterColumn<string>(
                name: "UnitOfProduction",
                table: "ProductionCapacityEstimations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "EffectiveProductionHour",
                table: "ProductionCapacityEstimations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "DesignProductionCapacity",
                table: "ProductionCapacityEstimations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "AttainableProductionCapacity",
                table: "ProductionCapacityEstimations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "CaseReferenceNumber",
                table: "ProductionCapacityEstimations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
