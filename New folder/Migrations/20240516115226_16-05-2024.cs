using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class _16052024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cases_CreateUsers_CaseOriginatordId",
                table: "Cases");

            migrationBuilder.DropIndex(
                name: "IX_Cases_CaseOriginatordId",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "CaseOriginatordId",
                table: "Cases");

            migrationBuilder.CreateIndex(
                name: "IX_Cases_CaseOriginatorId",
                table: "Cases",
                column: "CaseOriginatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_CreateUsers_CaseOriginatorId",
                table: "Cases",
                column: "CaseOriginatorId",
                principalTable: "CreateUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cases_CreateUsers_CaseOriginatorId",
                table: "Cases");

            migrationBuilder.DropIndex(
                name: "IX_Cases_CaseOriginatorId",
                table: "Cases");

            migrationBuilder.AddColumn<Guid>(
                name: "CaseOriginatordId",
                table: "Cases",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cases_CaseOriginatordId",
                table: "Cases",
                column: "CaseOriginatordId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_CreateUsers_CaseOriginatordId",
                table: "Cases",
                column: "CaseOriginatordId",
                principalTable: "CreateUsers",
                principalColumn: "Id");
        }
    }
}
