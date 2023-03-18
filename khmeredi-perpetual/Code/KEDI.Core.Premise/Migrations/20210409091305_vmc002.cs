using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class vmc002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KvmsInfoIDD",
                schema: "dbo",
                table: "tbKvmsInfo",
                newName: "KvmsInfoID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KvmsInfoID",
                schema: "dbo",
                table: "tbKvmsInfo",
                newName: "KvmsInfoIDD");
        }
    }
}
