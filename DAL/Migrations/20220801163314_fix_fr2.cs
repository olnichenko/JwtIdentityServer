using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class fix_fr2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResetPasswordKey_Users_UserId",
                table: "ResetPasswordKey");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "ResetPasswordKey",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ResetPasswordKey_Users_UserId",
                table: "ResetPasswordKey",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResetPasswordKey_Users_UserId",
                table: "ResetPasswordKey");

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
    }
}
