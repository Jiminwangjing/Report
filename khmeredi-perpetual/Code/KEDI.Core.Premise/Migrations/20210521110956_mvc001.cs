using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class mvc001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbUserAccount_tbBranch_BranchID",
                schema: "dbo",
                table: "tbUserAccount");

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativeValue",
                schema: "dbo",
                table: "tbWarehouseSummary",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "BranchID",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tbUserAccount_tbBranch_BranchID",
                schema: "dbo",
                table: "tbUserAccount",
                column: "BranchID",
                principalSchema: "dbo",
                principalTable: "tbBranch",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbUserAccount_tbBranch_BranchID",
                schema: "dbo",
                table: "tbUserAccount");

            migrationBuilder.DropColumn(
                name: "CumulativeValue",
                schema: "dbo",
                table: "tbWarehouseSummary");

            migrationBuilder.AlterColumn<int>(
                name: "BranchID",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_tbUserAccount_tbBranch_BranchID",
                schema: "dbo",
                table: "tbUserAccount",
                column: "BranchID",
                principalSchema: "dbo",
                principalTable: "tbBranch",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
