using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _176 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientApi");

            migrationBuilder.DropTable(
                name: "ClientSyncHistory");

            migrationBuilder.DropTable(
                name: "ServerSyncHistory");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiptNo",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReceiptNo",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ClientApi",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AppId = table.Column<string>(nullable: true),
                    ClientCode = table.Column<string>(nullable: true),
                    ClientName = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    IpAddress = table.Column<string>(nullable: true),
                    PublicKey = table.Column<string>(nullable: true),
                    Readonly = table.Column<bool>(nullable: false),
                    Revoked = table.Column<bool>(nullable: false),
                    Signature = table.Column<string>(nullable: true),
                    StrictIpAddress = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientApi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientSyncHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AppId = table.Column<string>(nullable: true),
                    ChangeLog = table.Column<DateTimeOffset>(nullable: false),
                    RowId = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TenantId = table.Column<string>(nullable: true),
                    TxId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSyncHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerSyncHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AppId = table.Column<string>(nullable: true),
                    ChangeLog = table.Column<DateTimeOffset>(nullable: false),
                    RowId = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TenantId = table.Column<string>(nullable: true),
                    TxId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerSyncHistory", x => x.Id);
                });
        }
    }
}
