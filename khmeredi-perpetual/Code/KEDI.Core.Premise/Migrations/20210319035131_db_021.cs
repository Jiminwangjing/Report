using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GLAccID",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "tbAutoBrand",
                schema: "dbo",
                columns: table => new
                {
                    BrandID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BrandName = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbAutoBrand", x => x.BrandID);
                });

            migrationBuilder.CreateTable(
                name: "tbAutoColor",
                schema: "dbo",
                columns: table => new
                {
                    ColorID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ColorName = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbAutoColor", x => x.ColorID);
                });

            migrationBuilder.CreateTable(
                name: "tbAutoMobile",
                schema: "dbo",
                columns: table => new
                {
                    AutoMID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusinessPartnerID = table.Column<int>(nullable: false),
                    Plate = table.Column<string>(nullable: true),
                    Frame = table.Column<string>(nullable: true),
                    Engine = table.Column<string>(nullable: true),
                    TypeID = table.Column<int>(nullable: false),
                    BrandID = table.Column<int>(nullable: false),
                    ModelID = table.Column<int>(nullable: false),
                    ColorID = table.Column<int>(nullable: false),
                    Year = table.Column<string>(nullable: true),
                    Deleted = table.Column<bool>(nullable: false),
                    KeyID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbAutoMobile", x => x.AutoMID);
                    table.ForeignKey(
                        name: "FK_tbAutoMobile_tbBusinessPartner_BusinessPartnerID",
                        column: x => x.BusinessPartnerID,
                        principalSchema: "dbo",
                        principalTable: "tbBusinessPartner",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbAutoModel",
                schema: "dbo",
                columns: table => new
                {
                    ModelID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ModelName = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbAutoModel", x => x.ModelID);
                });

            migrationBuilder.CreateTable(
                name: "tbAutoType",
                schema: "dbo",
                columns: table => new
                {
                    TypeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TypeName = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbAutoType", x => x.TypeID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbAutoMobile_BusinessPartnerID",
                schema: "dbo",
                table: "tbAutoMobile",
                column: "BusinessPartnerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbAutoBrand",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbAutoColor",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbAutoMobile",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbAutoModel",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbAutoType",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "GLAccID",
                schema: "dbo",
                table: "tbBusinessPartner");
        }
    }
}
