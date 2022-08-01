using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class fix_fr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResetPasswordKey_Users_userId",
                table: "ResetPasswordKey");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "ResetPasswordKey",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ResetPasswordKey_userId",
                table: "ResetPasswordKey",
                newName: "IX_ResetPasswordKey_UserId");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "ResetPasswordKey",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_ResetPasswordKey_Users_UserId",
                table: "ResetPasswordKey",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResetPasswordKey_Users_UserId",
                table: "ResetPasswordKey");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ResetPasswordKey",
                newName: "userId");

            migrationBuilder.RenameIndex(
                name: "IX_ResetPasswordKey_UserId",
                table: "ResetPasswordKey",
                newName: "IX_ResetPasswordKey_userId");

            migrationBuilder.AlterColumn<long>(
                name: "userId",
                table: "ResetPasswordKey",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ResetPasswordKey_Users_userId",
                table: "ResetPasswordKey",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
