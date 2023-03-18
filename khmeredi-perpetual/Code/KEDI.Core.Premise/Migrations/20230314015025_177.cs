using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _177 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientApp",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    AppId = table.Column<string>(nullable: true),
                    IpAddress = table.Column<string>(nullable: true),
                    Signature = table.Column<string>(nullable: true),
                    PublicKey = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    StrictIpAddress = table.Column<bool>(nullable: false),
                    IsReadonly = table.Column<bool>(nullable: false),
                    IsRevoked = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Environment = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientApp", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientSyncHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowId = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ChangeLog = table.Column<DateTimeOffset>(nullable: false),
                    TableName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSyncHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerSyncHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<long>(nullable: false),
                    TenantId = table.Column<string>(nullable: true),
                    RowId = table.Column<Guid>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    ChangeLog = table.Column<DateTimeOffset>(nullable: false),
                    TableName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerSyncHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientApp");

            migrationBuilder.DropTable(
                name: "ClientSyncHistory");

            migrationBuilder.DropTable(
                name: "ServerSyncHistory");
        }
    }
}
