using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class New2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeInterval_PCEEvaluations_PCEEvaluationId",
                table: "TimeInterval");

            migrationBuilder.DropIndex(
                name: "IX_TimeInterval_PCEEvaluationId",
                table: "TimeInterval");

            migrationBuilder.DropColumn(
                name: "PCEEvaluationId",
                table: "TimeInterval");

            migrationBuilder.CreateIndex(
                name: "IX_TimeInterval_PCEEId",
                table: "TimeInterval",
                column: "PCEEId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeInterval_PCEEvaluations_PCEEId",
                table: "TimeInterval",
                column: "PCEEId",
                principalTable: "PCEEvaluations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeInterval_PCEEvaluations_PCEEId",
                table: "TimeInterval");

            migrationBuilder.DropIndex(
                name: "IX_TimeInterval_PCEEId",
                table: "TimeInterval");

            migrationBuilder.AddColumn<Guid>(
                name: "PCEEvaluationId",
                table: "TimeInterval",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeInterval_PCEEvaluationId",
                table: "TimeInterval",
                column: "PCEEvaluationId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeInterval_PCEEvaluations_PCEEvaluationId",
                table: "TimeInterval",
                column: "PCEEvaluationId",
                principalTable: "PCEEvaluations",
                principalColumn: "Id");
        }
    }
}
