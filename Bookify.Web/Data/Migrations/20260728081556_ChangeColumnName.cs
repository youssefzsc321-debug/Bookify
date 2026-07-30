using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Areas_Governorates_GovernreteId",
                table: "Areas");

            migrationBuilder.RenameColumn(
                name: "GovernreteId",
                table: "Areas",
                newName: "GovernorateId");

            migrationBuilder.RenameIndex(
                name: "IX_Areas_Name_GovernreteId",
                table: "Areas",
                newName: "IX_Areas_Name_GovernorateId");

            migrationBuilder.RenameIndex(
                name: "IX_Areas_GovernreteId",
                table: "Areas",
                newName: "IX_Areas_GovernorateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Areas_Governorates_GovernorateId",
                table: "Areas",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Areas_Governorates_GovernorateId",
                table: "Areas");

            migrationBuilder.RenameColumn(
                name: "GovernorateId",
                table: "Areas",
                newName: "GovernreteId");

            migrationBuilder.RenameIndex(
                name: "IX_Areas_Name_GovernorateId",
                table: "Areas",
                newName: "IX_Areas_Name_GovernreteId");

            migrationBuilder.RenameIndex(
                name: "IX_Areas_GovernorateId",
                table: "Areas",
                newName: "IX_Areas_GovernreteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Areas_Governorates_GovernreteId",
                table: "Areas",
                column: "GovernreteId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
