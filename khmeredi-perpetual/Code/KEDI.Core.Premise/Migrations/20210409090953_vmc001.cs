using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class vmc001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbReceiptKvms_tbKvmsInfo_KvmsInfoID",
                schema: "dbo",
                table: "tbReceiptKvms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbKvmsInfo",
                schema: "dbo",
                table: "tbKvmsInfo");

            migrationBuilder.DropColumn(
                name: "KvmsInfoID",
                schema: "dbo",
                table: "tbKvmsInfo");

            migrationBuilder.AddColumn<int>(
                name: "SeriesDID",
                schema: "dbo",
                table: "tbReceiptKvms",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesID",
                schema: "dbo",
                table: "tbReceiptKvms",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KvmsInfoIDD",
                schema: "dbo",
                table: "tbKvmsInfo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbKvmsInfo",
                schema: "dbo",
                table: "tbKvmsInfo",
                column: "KvmsInfoIDD");

            migrationBuilder.AddForeignKey(
                name: "FK_tbReceiptKvms_tbKvmsInfo_KvmsInfoID",
                schema: "dbo",
                table: "tbReceiptKvms",
                column: "KvmsInfoID",
                principalSchema: "dbo",
                principalTable: "tbKvmsInfo",
                principalColumn: "KvmsInfoIDD",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbReceiptKvms_tbKvmsInfo_KvmsInfoID",
                schema: "dbo",
                table: "tbReceiptKvms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbKvmsInfo",
                schema: "dbo",
                table: "tbKvmsInfo");

            migrationBuilder.DropColumn(
                name: "SeriesDID",
                schema: "dbo",
                table: "tbReceiptKvms");

            migrationBuilder.DropColumn(
                name: "SeriesID",
                schema: "dbo",
                table: "tbReceiptKvms");

            migrationBuilder.DropColumn(
                name: "KvmsInfoIDD",
                schema: "dbo",
                table: "tbKvmsInfo");

            migrationBuilder.AddColumn<int>(
                name: "KvmsInfoID",
                schema: "dbo",
                table: "tbKvmsInfo",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbKvmsInfo",
                schema: "dbo",
                table: "tbKvmsInfo",
                column: "KvmsInfoID");

            migrationBuilder.AddForeignKey(
                name: "FK_tbReceiptKvms_tbKvmsInfo_KvmsInfoID",
                schema: "dbo",
                table: "tbReceiptKvms",
                column: "KvmsInfoID",
                principalSchema: "dbo",
                principalTable: "tbKvmsInfo",
                principalColumn: "KvmsInfoID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
