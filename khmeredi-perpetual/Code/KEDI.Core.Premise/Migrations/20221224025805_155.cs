using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _155 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantTransaction");

            migrationBuilder.AddColumn<bool>(
                name: "Skin",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "tbFontSetting",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SkinItem",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SkinID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    Unable = table.Column<bool>(nullable: false),
                    SkinName = table.Column<string>(nullable: true),
                    BackgroundColor = table.Column<string>(nullable: true),
                    AlphabetColor = table.Column<string>(nullable: true),
                    HoverColor = table.Column<string>(nullable: true),
                    designName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkinItem", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SkinUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SkinID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    Unable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkinUser", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TransactionAeon",
                columns: table => new
                {
                    RowId = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SyncDate = table.Column<DateTime>(nullable: false),
                    TransactionId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionAeon", x => x.RowId);
                });

            migrationBuilder.CreateTable(
                name: "TransactionChipMong",
                columns: table => new
                {
                    RowId = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SyncDate = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<string>(nullable: true),
                    PosId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionChipMong", x => x.RowId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SkinItem");

            migrationBuilder.DropTable(
                name: "SkinUser");

            migrationBuilder.DropTable(
                name: "TransactionAeon");

            migrationBuilder.DropTable(
                name: "TransactionChipMong");

            migrationBuilder.DropColumn(
                name: "Skin",
                schema: "dbo",
                table: "tbUserAccount");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "tbFontSetting");

            migrationBuilder.CreateTable(
                name: "TenantTransaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RowId = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SyncDate = table.Column<DateTime>(nullable: false),
                    Synced = table.Column<bool>(nullable: false),
                    TenantId = table.Column<string>(nullable: true),
                    TransactId = table.Column<string>(nullable: true),
                    TransactType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantTransaction", x => x.Id);
                });
        }
    }
}
