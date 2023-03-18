using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_059 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BaseOnID",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CopyType",
                schema: "dbo",
                table: "tbGoodsReciptPO",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseOnID",
                schema: "dbo",
                table: "tbGoodsReciptPO");

            migrationBuilder.DropColumn(
                name: "CopyType",
                schema: "dbo",
                table: "tbGoodsReciptPO");
        }
    }
}
