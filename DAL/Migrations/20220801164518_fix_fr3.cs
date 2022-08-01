using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class fix_fr3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResetPasswordKey_Users_UserId",
                table: "ResetPasswordKey");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResetPasswordKey",
                table: "ResetPasswordKey");

            migrationBuilder.RenameTable(
                name: "ResetPasswordKey",
                newName: "ResetPasswordKeys");

            migrationBuilder.RenameIndex(
                name: "IX_ResetPasswordKey_UserId",
                table: "ResetPasswordKeys",
                newName: "IX_ResetPasswordKeys_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResetPasswordKeys",
                table: "ResetPasswordKeys",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResetPasswordKeys_Users_UserId",
                table: "ResetPasswordKeys",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResetPasswordKeys_Users_UserId",
                table: "ResetPasswordKeys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResetPasswordKeys",
                table: "ResetPasswordKeys");

            migrationBuilder.RenameTable(
                name: "ResetPasswordKeys",
                newName: "ResetPasswordKey");

            migrationBuilder.RenameIndex(
                name: "IX_ResetPasswordKeys_UserId",
                table: "ResetPasswordKey",
                newName: "IX_ResetPasswordKey_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResetPasswordKey",
                table: "ResetPasswordKey",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResetPasswordKey_Users_UserId",
                table: "ResetPasswordKey",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
