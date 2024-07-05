using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSomeFieldsAfterComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PCEEvaluations_CreateUsers_CheckerID",
                table: "PCEEvaluations");

            migrationBuilder.DropColumn(
                name: "OutputType",
                table: "PCEEvaluations");

            migrationBuilder.RenameColumn(
                name: "CheckerID",
                table: "PCEEvaluations",
                newName: "CheckerId");

            migrationBuilder.RenameIndex(
                name: "IX_PCEEvaluations_CheckerID",
                table: "PCEEvaluations",
                newName: "IX_PCEEvaluations_CheckerId");

            migrationBuilder.AlterColumn<string>(
                name: "TechnicalObsolescenceStatus",
                table: "PCEEvaluations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OverallActualCurrentPlantCapacity",
                table: "PCEEvaluations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EstimatedProductionCapacity",
                table: "PCEEvaluations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DesignProductionCapacity",
                table: "PCEEvaluations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BottleneckProductionLineCapacity",
                table: "PCEEvaluations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AttainableProductionCapacity",
                table: "PCEEvaluations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ActualProductionCapacity",
                table: "PCEEvaluations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PCEEvaluations_CreateUsers_CheckerId",
                table: "PCEEvaluations",
                column: "CheckerId",
                principalTable: "CreateUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PCEEvaluations_CreateUsers_CheckerId",
                table: "PCEEvaluations");

            migrationBuilder.RenameColumn(
                name: "CheckerId",
                table: "PCEEvaluations",
                newName: "CheckerID");

            migrationBuilder.RenameIndex(
                name: "IX_PCEEvaluations_CheckerId",
                table: "PCEEvaluations",
                newName: "IX_PCEEvaluations_CheckerID");

            migrationBuilder.AlterColumn<int>(
                name: "TechnicalObsolescenceStatus",
                table: "PCEEvaluations",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OverallActualCurrentPlantCapacity",
                table: "PCEEvaluations",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "EstimatedProductionCapacity",
                table: "PCEEvaluations",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DesignProductionCapacity",
                table: "PCEEvaluations",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BottleneckProductionLineCapacity",
                table: "PCEEvaluations",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AttainableProductionCapacity",
                table: "PCEEvaluations",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ActualProductionCapacity",
                table: "PCEEvaluations",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OutputType",
                table: "PCEEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PCEEvaluations_CreateUsers_CheckerID",
                table: "PCEEvaluations",
                column: "CheckerID",
                principalTable: "CreateUsers",
                principalColumn: "Id");
        }
    }
}
