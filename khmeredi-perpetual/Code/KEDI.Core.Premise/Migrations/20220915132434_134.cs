using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _134 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MultiIncommings",
                table: "MultiIncommings");

            migrationBuilder.RenameTable(
                name: "MultiIncommings",
                newName: "MultPayIncomming");

            migrationBuilder.RenameColumn(
                name: "IncomingID",
                table: "MultPayIncomming",
                newName: "PaymentMeanID");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                schema: "dbo",
                table: "tbUserAccount",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                schema: "dbo",
                table: "tbReceiptInformation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "dbo",
                table: "tbReceiptInformation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeamCondition2",
                schema: "dbo",
                table: "tbReceiptInformation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title2",
                schema: "dbo",
                table: "tbReceiptInformation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VatTin",
                schema: "dbo",
                table: "tbReceiptInformation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                schema: "dbo",
                table: "tbReceiptInformation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logo2",
                schema: "dbo",
                table: "tbCompany",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GLAccDepositID",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name2",
                schema: "dbo",
                table: "tbBranch",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmmountSys",
                table: "MultPayIncomming",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "IncomingPaymentID",
                table: "MultPayIncomming",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "SCRate",
                table: "MultPayIncomming",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MultPayIncomming",
                table: "MultPayIncomming",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_MultPayIncomming_IncomingPaymentID",
                table: "MultPayIncomming",
                column: "IncomingPaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_MultPayIncomming_tbIncomingPayment_IncomingPaymentID",
                table: "MultPayIncomming",
                column: "IncomingPaymentID",
                principalSchema: "dbo",
                principalTable: "tbIncomingPayment",
                principalColumn: "IncomingPaymentID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MultPayIncomming_tbIncomingPayment_IncomingPaymentID",
                table: "MultPayIncomming");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MultPayIncomming",
                table: "MultPayIncomming");

            migrationBuilder.DropIndex(
                name: "IX_MultPayIncomming_IncomingPaymentID",
                table: "MultPayIncomming");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                schema: "dbo",
                table: "tbUserAccount");

            migrationBuilder.DropColumn(
                name: "Address2",
                schema: "dbo",
                table: "tbReceiptInformation");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "dbo",
                table: "tbReceiptInformation");

            migrationBuilder.DropColumn(
                name: "TeamCondition2",
                schema: "dbo",
                table: "tbReceiptInformation");

            migrationBuilder.DropColumn(
                name: "Title2",
                schema: "dbo",
                table: "tbReceiptInformation");

            migrationBuilder.DropColumn(
                name: "VatTin",
                schema: "dbo",
                table: "tbReceiptInformation");

            migrationBuilder.DropColumn(
                name: "Website",
                schema: "dbo",
                table: "tbReceiptInformation");

            migrationBuilder.DropColumn(
                name: "Logo2",
                schema: "dbo",
                table: "tbCompany");

            migrationBuilder.DropColumn(
                name: "GLAccDepositID",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "Name2",
                schema: "dbo",
                table: "tbBranch");

            migrationBuilder.DropColumn(
                name: "AmmountSys",
                table: "MultPayIncomming");

            migrationBuilder.DropColumn(
                name: "IncomingPaymentID",
                table: "MultPayIncomming");

            migrationBuilder.DropColumn(
                name: "SCRate",
                table: "MultPayIncomming");

            migrationBuilder.RenameTable(
                name: "MultPayIncomming",
                newName: "MultiIncommings");

            migrationBuilder.RenameColumn(
                name: "PaymentMeanID",
                table: "MultiIncommings",
                newName: "IncomingID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MultiIncommings",
                table: "MultiIncommings",
                column: "ID");
        }
    }
}
