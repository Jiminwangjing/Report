using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbDocuNumbering",
                schema: "dbo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbDocuNumbering",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DefaultSeries = table.Column<string>(nullable: true),
                    DocuTypeID = table.Column<int>(nullable: false),
                    Document = table.Column<string>(nullable: true),
                    FirstNo = table.Column<string>(nullable: true),
                    LastNo = table.Column<string>(nullable: true),
                    NextNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbDocuNumbering", x => x.ID);
                });
        }
    }
}
