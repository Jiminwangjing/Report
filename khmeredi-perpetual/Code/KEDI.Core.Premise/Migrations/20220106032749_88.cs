using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _88 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BuyXAmGetXDisRate",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BuyXAmGetXDisType",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BuyXAmGetXDisValue",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BuyXAmountGetXDisID",
                schema: "dbo",
                table: "tbVoidOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BuyXAmGetXDisRate",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BuyXAmGetXDisType",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BuyXAmGetXDisValue",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BuyXAmountGetXDisID",
                schema: "dbo",
                table: "tbReceipt",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BuyXAmGetXDisRate",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BuyXAmGetXDisType",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BuyXAmGetXDisValue",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BuyXAmountGetXDisID",
                schema: "dbo",
                table: "tbOrder",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CardMemberID",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BuyXAmGetXDisRate",
                table: "VoidItem",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BuyXAmGetXDisType",
                table: "VoidItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BuyXAmGetXDisValue",
                table: "VoidItem",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BuyXAmountGetXDisID",
                table: "VoidItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BPAcctID",
                table: "tbAccountBalance",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Creator",
                table: "tbAccountBalance",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AccountMemberCardID",
                table: "SaleGLADeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BuyXAmGetXDisRate",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BuyXAmGetXDisType",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BuyXAmGetXDisValue",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BuyXAmountGetXDisID",
                table: "ReceiptMemo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AccountMemberCards",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CashAccID = table.Column<int>(nullable: false),
                    UnearnedRevenueID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountMemberCards", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BuyXQtyGetXDis",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    DateF = table.Column<DateTime>(nullable: false),
                    DateT = table.Column<DateTime>(nullable: false),
                    BuyItemID = table.Column<int>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    DisItemID = table.Column<int>(nullable: false),
                    DisRate = table.Column<decimal>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyXQtyGetXDis", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CardMemberDeposits",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CusID = table.Column<int>(nullable: false),
                    CardMemberID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    SeriesDID = table.Column<int>(nullable: false),
                    DocTypeID = table.Column<int>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    PostingDate = table.Column<DateTime>(nullable: false),
                    TotalDeposit = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardMemberDeposits", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CardMemberDepositTransactions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CardMemberDepositID = table.Column<int>(nullable: false),
                    CusID = table.Column<int>(nullable: false),
                    CardMemberID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    SeriesDID = table.Column<int>(nullable: false),
                    DocTypeID = table.Column<int>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    PostingDate = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    CumulativeBalance = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardMemberDepositTransactions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CardMembers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TypeCardID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardMembers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PBuyXGetXDis",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    PriListID = table.Column<int>(nullable: false),
                    DateF = table.Column<DateTime>(nullable: false),
                    DateT = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    DisType = table.Column<int>(nullable: false),
                    DisRateValue = table.Column<decimal>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PBuyXGetXDis", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TypeCards",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeCards", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleGLADeter_AccountMemberCardID",
                table: "SaleGLADeter",
                column: "AccountMemberCardID");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleGLADeter_AccountMemberCards_AccountMemberCardID",
                table: "SaleGLADeter",
                column: "AccountMemberCardID",
                principalTable: "AccountMemberCards",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleGLADeter_AccountMemberCards_AccountMemberCardID",
                table: "SaleGLADeter");

            migrationBuilder.DropTable(
                name: "AccountMemberCards");

            migrationBuilder.DropTable(
                name: "BuyXQtyGetXDis");

            migrationBuilder.DropTable(
                name: "CardMemberDeposits");

            migrationBuilder.DropTable(
                name: "CardMemberDepositTransactions");

            migrationBuilder.DropTable(
                name: "CardMembers");

            migrationBuilder.DropTable(
                name: "PBuyXGetXDis");

            migrationBuilder.DropTable(
                name: "TypeCards");

            migrationBuilder.DropIndex(
                name: "IX_SaleGLADeter_AccountMemberCardID",
                table: "SaleGLADeter");

            migrationBuilder.DropColumn(
                name: "BuyXAmGetXDisRate",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "BuyXAmGetXDisType",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "BuyXAmGetXDisValue",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "BuyXAmountGetXDisID",
                schema: "dbo",
                table: "tbVoidOrder");

            migrationBuilder.DropColumn(
                name: "BuyXAmGetXDisRate",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "BuyXAmGetXDisType",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "BuyXAmGetXDisValue",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "BuyXAmountGetXDisID",
                schema: "dbo",
                table: "tbReceipt");

            migrationBuilder.DropColumn(
                name: "BuyXAmGetXDisRate",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "BuyXAmGetXDisType",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "BuyXAmGetXDisValue",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "BuyXAmountGetXDisID",
                schema: "dbo",
                table: "tbOrder");

            migrationBuilder.DropColumn(
                name: "Balance",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "CardMemberID",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "BuyXAmGetXDisRate",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "BuyXAmGetXDisType",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "BuyXAmGetXDisValue",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "BuyXAmountGetXDisID",
                table: "VoidItem");

            migrationBuilder.DropColumn(
                name: "BPAcctID",
                table: "tbAccountBalance");

            migrationBuilder.DropColumn(
                name: "Creator",
                table: "tbAccountBalance");

            migrationBuilder.DropColumn(
                name: "AccountMemberCardID",
                table: "SaleGLADeter");

            migrationBuilder.DropColumn(
                name: "BuyXAmGetXDisRate",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "BuyXAmGetXDisType",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "BuyXAmGetXDisValue",
                table: "ReceiptMemo");

            migrationBuilder.DropColumn(
                name: "BuyXAmountGetXDisID",
                table: "ReceiptMemo");
        }
    }
}
