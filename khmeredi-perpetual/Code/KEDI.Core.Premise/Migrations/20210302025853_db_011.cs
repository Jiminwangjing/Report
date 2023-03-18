using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_011 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SubPeriod",
                schema: "dbo",
                table: "tbPotingPeriod",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NoOfPeroid",
                schema: "dbo",
                table: "tbPotingPeriod",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SubPeriod",
                schema: "dbo",
                table: "tbPotingPeriod",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "NoOfPeroid",
                schema: "dbo",
                table: "tbPotingPeriod",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
