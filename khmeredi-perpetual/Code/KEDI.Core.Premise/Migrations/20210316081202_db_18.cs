using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_18 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "firstNo",
                schema: "dbo",
                table: "tbSeries",
                newName: "FirstNo");

            migrationBuilder.RenameColumn(
                name: "firstNo",
                schema: "dbo",
                table: "tbDocuNumbering",
                newName: "FirstNo");

            migrationBuilder.AddColumn<string>(
                name: "PreFix",
                schema: "dbo",
                table: "tbSeries",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NoOfPeroid",
                schema: "dbo",
                table: "tbPotingPeriod",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "OriginNo",
                table: "tbAccountBalance",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreFix",
                schema: "dbo",
                table: "tbSeries");

            migrationBuilder.RenameColumn(
                name: "FirstNo",
                schema: "dbo",
                table: "tbSeries",
                newName: "firstNo");

            migrationBuilder.RenameColumn(
                name: "FirstNo",
                schema: "dbo",
                table: "tbDocuNumbering",
                newName: "firstNo");

            migrationBuilder.AlterColumn<int>(
                name: "NoOfPeroid",
                schema: "dbo",
                table: "tbPotingPeriod",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OriginNo",
                table: "tbAccountBalance",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
