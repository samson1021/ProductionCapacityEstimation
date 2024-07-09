using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mechanical.Migrations
{
    /// <inheritdoc />
    public partial class Changedfunctionalitylabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OtherMachineFunctionalityReason",
                table: "PCEEvaluations",
                newName: "OtherMachineNonFunctionalityReason");

            migrationBuilder.RenameColumn(
                name: "MachineFunctionalityReason",
                table: "PCEEvaluations",
                newName: "MachineNonFunctionalityReason");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OtherMachineNonFunctionalityReason",
                table: "PCEEvaluations",
                newName: "OtherMachineFunctionalityReason");

            migrationBuilder.RenameColumn(
                name: "MachineNonFunctionalityReason",
                table: "PCEEvaluations",
                newName: "MachineFunctionalityReason");
        }
    }
}
