using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class PCEEvaluationUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PCEEvaluations_PCECases_PCECaseId",
                table: "PCEEvaluations");

            migrationBuilder.RenameColumn(
                name: "PCECaseId",
                table: "PCEEvaluations",
                newName: "PCEId");

            migrationBuilder.RenameIndex(
                name: "IX_PCEEvaluations_PCECaseId",
                table: "PCEEvaluations",
                newName: "IX_PCEEvaluations_PCEId");

            migrationBuilder.AddColumn<Guid>(
                name: "CheckerID",
                table: "PCEEvaluations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EvaluatorID",
                table: "PCEEvaluations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PCEEvaluations_CheckerID",
                table: "PCEEvaluations",
                column: "CheckerID");

            migrationBuilder.CreateIndex(
                name: "IX_PCEEvaluations_EvaluatorID",
                table: "PCEEvaluations",
                column: "EvaluatorID");

            migrationBuilder.AddForeignKey(
                name: "FK_PCEEvaluations_CreateUsers_CheckerID",
                table: "PCEEvaluations",
                column: "CheckerID",
                principalTable: "CreateUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PCEEvaluations_CreateUsers_EvaluatorID",
                table: "PCEEvaluations",
                column: "EvaluatorID",
                principalTable: "CreateUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PCEEvaluations_PCECases_PCEId",
                table: "PCEEvaluations",
                column: "PCEId",
                principalTable: "PCECases",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PCEEvaluations_CreateUsers_CheckerID",
                table: "PCEEvaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_PCEEvaluations_CreateUsers_EvaluatorID",
                table: "PCEEvaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_PCEEvaluations_PCECases_PCEId",
                table: "PCEEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_PCEEvaluations_CheckerID",
                table: "PCEEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_PCEEvaluations_EvaluatorID",
                table: "PCEEvaluations");

            migrationBuilder.DropColumn(
                name: "CheckerID",
                table: "PCEEvaluations");

            migrationBuilder.DropColumn(
                name: "EvaluatorID",
                table: "PCEEvaluations");

            migrationBuilder.RenameColumn(
                name: "PCEId",
                table: "PCEEvaluations",
                newName: "PCECaseId");

            migrationBuilder.RenameIndex(
                name: "IX_PCEEvaluations_PCEId",
                table: "PCEEvaluations",
                newName: "IX_PCEEvaluations_PCECaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_PCEEvaluations_PCECases_PCECaseId",
                table: "PCEEvaluations",
                column: "PCECaseId",
                principalTable: "PCECases",
                principalColumn: "Id");
        }
    }
}
