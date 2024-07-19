using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class editmigtationonassignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionCaseAssignments_ProductionCapacities_ProductionCapacityId",
                table: "ProductionCaseAssignments");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionCaseAssignments_ProductionCapacities_ProductionCapacityId",
                table: "ProductionCaseAssignments",
                column: "ProductionCapacityId",
                principalTable: "ProductionCapacities",
                principalColumn: "Id");
        }
    }
}
