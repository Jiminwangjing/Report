using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _153 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ColorSetting",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Checked = table.Column<bool>(nullable: false),
                    checkClick = table.Column<string>(nullable: true),
                    BackgroundColor = table.Column<string>(nullable: true),
                    FontColor = table.Column<string>(nullable: true),
                    BackgroundMenu = table.Column<string>(nullable: true),
                    BackOfSubmenu = table.Column<string>(nullable: true),
                    HoverBackgSubmenu = table.Column<string>(nullable: true),
                    BacksubmenuOnItem = table.Column<string>(nullable: true),
                    BackgButton = table.Column<string>(nullable: true),
                    Backgtableth = table.Column<string>(nullable: true),
                    Backgtabletd = table.Column<string>(nullable: true),
                    backgroundInput = table.Column<string>(nullable: true),
                    BackgroundCard = table.Column<string>(nullable: true),
                    BackgroundBar = table.Column<string>(nullable: true),
                    BackgroundBarItem = table.Column<string>(nullable: true),
                    BackgSlideBarTitle = table.Column<string>(nullable: true),
                    BackgroundIcon = table.Column<string>(nullable: true),
                    BackroundIconImage = table.Column<string>(nullable: true),
                    BackgBodyCard = table.Column<string>(nullable: true),
                    BackgBodyWet = table.Column<string>(nullable: true),
                    BackgBodyWetCard = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorSetting", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbFontSetting",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FontName = table.Column<string>(nullable: true),
                    FontSize = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbFontSetting", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColorSetting");

            migrationBuilder.DropTable(
                name: "tbFontSetting");
        }
    }
}
