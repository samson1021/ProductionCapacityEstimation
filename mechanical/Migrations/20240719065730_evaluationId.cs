using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class evaluationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadFiles_PCEEvaluations_PCEEvaluationId",
                table: "UploadFiles");

            migrationBuilder.DropIndex(
                name: "IX_UploadFiles_PCEEvaluationId",
                table: "UploadFiles");

            migrationBuilder.DropColumn(
                name: "PCEEvaluationId",
                table: "UploadFiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PCEEvaluationId",
                table: "UploadFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UploadFiles_PCEEvaluationId",
                table: "UploadFiles",
                column: "PCEEvaluationId");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadFiles_PCEEvaluations_PCEEvaluationId",
                table: "UploadFiles",
                column: "PCEEvaluationId",
                principalTable: "PCEEvaluations",
                principalColumn: "Id");
        }
    }
}
