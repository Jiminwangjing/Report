using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _104 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAllowNegativeStock",
                schema: "dbo",
                table: "tbWarhouse",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDateFrom",
                table: "CardMembers",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDateTo",
                table: "CardMembers",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "LengthExpireCard",
                table: "CardMembers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RenewCardHistory",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CardID = table.Column<int>(nullable: false),
                    CusID = table.Column<int>(nullable: false),
                    LastDateExpirationFrom = table.Column<DateTime>(type: "Date", nullable: false),
                    LastDateExpirationTo = table.Column<DateTime>(type: "Date", nullable: false),
                    RenewDateFrom = table.Column<DateTime>(type: "Date", nullable: false),
                    RenewDateTo = table.Column<DateTime>(type: "Date", nullable: false),
                    LengthExpireCard = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RenewCardHistory", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RenewCardHistory");

            migrationBuilder.DropColumn(
                name: "IsAllowNegativeStock",
                schema: "dbo",
                table: "tbWarhouse");

            migrationBuilder.DropColumn(
                name: "ExpireDateFrom",
                table: "CardMembers");

            migrationBuilder.DropColumn(
                name: "ExpireDateTo",
                table: "CardMembers");

            migrationBuilder.DropColumn(
                name: "LengthExpireCard",
                table: "CardMembers");
        }
    }
}
