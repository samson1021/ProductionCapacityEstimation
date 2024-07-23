using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class cleaned : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PCEEvaluations_DateTimePeriod_TimeConsumedToCheckId",
                table: "PCEEvaluations");

            migrationBuilder.DropTable(
                name: "DatePeriod");

            migrationBuilder.DropTable(
                name: "DateTimePeriod");

            migrationBuilder.DropTable(
                name: "TimePeriod");

            migrationBuilder.CreateTable(
                name: "DateTimeRange",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DateTimeRange", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeRange",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Start = table.Column<TimeSpan>(type: "time", nullable: false),
                    End = table.Column<TimeSpan>(type: "time", nullable: false),
                    PCEEvaluationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeRange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeRange_PCEEvaluations_PCEEvaluationId",
                        column: x => x.PCEEvaluationId,
                        principalTable: "PCEEvaluations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeRange_PCEEvaluationId",
                table: "TimeRange",
                column: "PCEEvaluationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DateTimeRange");

            migrationBuilder.DropTable(
                name: "TimeRange");

            migrationBuilder.CreateTable(
                name: "DatePeriod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatePeriod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DateTimePeriod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DateTimePeriod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimePeriod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    End = table.Column<TimeSpan>(type: "time", nullable: false),
                    PCEEvaluationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Start = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimePeriod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimePeriod_PCEEvaluations_PCEEvaluationId",
                        column: x => x.PCEEvaluationId,
                        principalTable: "PCEEvaluations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimePeriod_PCEEvaluationId",
                table: "TimePeriod",
                column: "PCEEvaluationId");

            migrationBuilder.AddForeignKey(
                name: "FK_PCEEvaluations_DateTimePeriod_TimeConsumedToCheckId",
                table: "PCEEvaluations",
                column: "TimeConsumedToCheckId",
                principalTable: "DateTimePeriod",
                principalColumn: "Id");
        }
    }
}
