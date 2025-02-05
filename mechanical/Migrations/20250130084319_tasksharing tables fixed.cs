using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class tasksharingtablesfixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Reject",
                table: "Reject");

            migrationBuilder.RenameTable(
                name: "Reject",
                newName: "Rejects");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rejects",
                table: "Rejects",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TaskManagments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaseOrginatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SharingReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaskStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PriorityType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskManagments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskManagments_CreateUsers_AssignedId",
                        column: x => x.AssignedId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskManagments_CreateUsers_CaseOrginatorId",
                        column: x => x.CaseOrginatorId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskNotifications_CreateUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CommentById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskComments_CreateUsers_CommentById",
                        column: x => x.CommentById,
                        principalTable: "CreateUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskComments_TaskManagments_TaskId",
                        column: x => x.TaskId,
                        principalTable: "TaskManagments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskComments_CommentById",
                table: "TaskComments",
                column: "CommentById");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComments_TaskId",
                table: "TaskComments",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskManagments_AssignedId",
                table: "TaskManagments",
                column: "AssignedId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskManagments_CaseOrginatorId",
                table: "TaskManagments",
                column: "CaseOrginatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskNotifications_UserId",
                table: "TaskNotifications",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskComments");

            migrationBuilder.DropTable(
                name: "TaskNotifications");

            migrationBuilder.DropTable(
                name: "TaskManagments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rejects",
                table: "Rejects");

            migrationBuilder.RenameTable(
                name: "Rejects",
                newName: "Reject");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reject",
                table: "Reject",
                column: "Id");
        }
    }
}
