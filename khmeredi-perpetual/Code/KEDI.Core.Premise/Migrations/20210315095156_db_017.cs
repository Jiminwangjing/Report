using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_017 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbGLAccount_tbAccountBalance_AccountBalanceID",
                table: "tbGLAccount");

            migrationBuilder.DropIndex(
                name: "IX_tbGLAccount_AccountBalanceID",
                table: "tbGLAccount");

            migrationBuilder.DropColumn(
                name: "AccountBalanceID",
                table: "tbGLAccount");

            migrationBuilder.AddColumn<int>(
                name: "GLAID",
                table: "tbAccountBalance",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GLAID",
                table: "tbAccountBalance");

            migrationBuilder.AddColumn<int>(
                name: "AccountBalanceID",
                table: "tbGLAccount",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbGLAccount_AccountBalanceID",
                table: "tbGLAccount",
                column: "AccountBalanceID");

            migrationBuilder.AddForeignKey(
                name: "FK_tbGLAccount_tbAccountBalance_AccountBalanceID",
                table: "tbGLAccount",
                column: "AccountBalanceID",
                principalTable: "tbAccountBalance",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
