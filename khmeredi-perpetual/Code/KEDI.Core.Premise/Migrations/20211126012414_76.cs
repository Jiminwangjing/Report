using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _76 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicKey",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Signature",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuthorizationTemplate",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Option = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizationTemplate", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorizationTemplate");

            migrationBuilder.DropColumn(
                name: "PublicKey",
                schema: "dbo",
                table: "tbUserAccount");

            migrationBuilder.DropColumn(
                name: "Signature",
                schema: "dbo",
                table: "tbUserAccount");
        }
    }
}
