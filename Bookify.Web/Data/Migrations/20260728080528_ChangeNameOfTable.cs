using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNameOfTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Areas_Governretes_GovernreteId",
                table: "Areas");

            migrationBuilder.DropForeignKey(
                name: "FK_Governretes_AspNetUsers_CreatedById",
                table: "Governretes");

            migrationBuilder.DropForeignKey(
                name: "FK_Governretes_AspNetUsers_LastUpdatedById",
                table: "Governretes");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscripers_Governretes_GovernreteId",
                table: "Subscripers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Governretes",
                table: "Governretes");

            migrationBuilder.RenameTable(
                name: "Governretes",
                newName: "Governorates");

            migrationBuilder.RenameIndex(
                name: "IX_Governretes_Name",
                table: "Governorates",
                newName: "IX_Governorates_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Governretes_LastUpdatedById",
                table: "Governorates",
                newName: "IX_Governorates_LastUpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Governretes_CreatedById",
                table: "Governorates",
                newName: "IX_Governorates_CreatedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Governorates",
                table: "Governorates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Areas_Governorates_GovernreteId",
                table: "Areas",
                column: "GovernreteId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Governorates_AspNetUsers_CreatedById",
                table: "Governorates",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Governorates_AspNetUsers_LastUpdatedById",
                table: "Governorates",
                column: "LastUpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscripers_Governorates_GovernreteId",
                table: "Subscripers",
                column: "GovernreteId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Areas_Governorates_GovernreteId",
                table: "Areas");

            migrationBuilder.DropForeignKey(
                name: "FK_Governorates_AspNetUsers_CreatedById",
                table: "Governorates");

            migrationBuilder.DropForeignKey(
                name: "FK_Governorates_AspNetUsers_LastUpdatedById",
                table: "Governorates");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscripers_Governorates_GovernreteId",
                table: "Subscripers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Governorates",
                table: "Governorates");

            migrationBuilder.RenameTable(
                name: "Governorates",
                newName: "Governretes");

            migrationBuilder.RenameIndex(
                name: "IX_Governorates_Name",
                table: "Governretes",
                newName: "IX_Governretes_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Governorates_LastUpdatedById",
                table: "Governretes",
                newName: "IX_Governretes_LastUpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Governorates_CreatedById",
                table: "Governretes",
                newName: "IX_Governretes_CreatedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Governretes",
                table: "Governretes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Areas_Governretes_GovernreteId",
                table: "Areas",
                column: "GovernreteId",
                principalTable: "Governretes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Governretes_AspNetUsers_CreatedById",
                table: "Governretes",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Governretes_AspNetUsers_LastUpdatedById",
                table: "Governretes",
                column: "LastUpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscripers_Governretes_GovernreteId",
                table: "Subscripers",
                column: "GovernreteId",
                principalTable: "Governretes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
