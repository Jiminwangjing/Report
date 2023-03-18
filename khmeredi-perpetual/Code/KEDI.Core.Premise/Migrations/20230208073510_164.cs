using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _164 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "TransactionAeon",
                newName: "TxId");

            migrationBuilder.AddColumn<string>(
                name: "TxId",
                table: "TransactionChipMong",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TxId",
                table: "TransactionChipMong");

            migrationBuilder.RenameColumn(
                name: "TxId",
                table: "TransactionAeon",
                newName: "TransactionId");
        }
    }
}
