using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_016 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountBalanceID",
                table: "tbGLAccount",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tbAccountBalance",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Origin = table.Column<int>(nullable: false),
                    OriginNo = table.Column<int>(nullable: false),
                    OffsetAccount = table.Column<string>(nullable: true),
                    Details = table.Column<string>(nullable: true),
                    CumulativeBalance = table.Column<decimal>(nullable: false),
                    Debit = table.Column<decimal>(nullable: false),
                    Credit = table.Column<decimal>(nullable: false),
                    LocalSetRate = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbAccountBalance", x => x.ID);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbGLAccount_tbAccountBalance_AccountBalanceID",
                table: "tbGLAccount");

            migrationBuilder.DropTable(
                name: "tbAccountBalance");

            migrationBuilder.DropIndex(
                name: "IX_tbGLAccount_AccountBalanceID",
                table: "tbGLAccount");

            migrationBuilder.DropColumn(
                name: "AccountBalanceID",
                table: "tbGLAccount");
        }
    }
}
