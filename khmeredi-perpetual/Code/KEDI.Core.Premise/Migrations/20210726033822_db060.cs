using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db060 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrintPurchaseAP",
                schema: "dbo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrintPurchaseAP",
                schema: "dbo",
                columns: table => new
                {
                    Brand = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    AddressEng = table.Column<string>(nullable: true),
                    Addresskh = table.Column<string>(nullable: true),
                    Applied_Amount = table.Column<double>(nullable: false),
                    Balance_Due_Local = table.Column<double>(nullable: false),
                    Balance_Due_Sys = table.Column<double>(nullable: false),
                    BaseOn = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true),
                    CusCode = table.Column<string>(nullable: true),
                    DiscountRate = table.Column<double>(nullable: false),
                    DiscountRate_Detail = table.Column<double>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    DiscountValue_Detail = table.Column<double>(nullable: false),
                    DocumentDate = table.Column<string>(nullable: true),
                    DueDate = table.Column<string>(nullable: true),
                    EnglishName = table.Column<string>(nullable: true),
                    ExchangeRate = table.Column<double>(nullable: false),
                    Invoice = table.Column<string>(nullable: true),
                    KhmerName = table.Column<string>(nullable: true),
                    LocalCurrency = table.Column<string>(nullable: true),
                    Logo = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    PostingDate = table.Column<string>(nullable: true),
                    PreFix = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    PurchasID = table.Column<int>(nullable: false),
                    Qty = table.Column<double>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    SQN = table.Column<string>(nullable: true),
                    Sub_Total = table.Column<double>(nullable: false),
                    Sub_Total_Detail = table.Column<double>(nullable: false),
                    Sub_Total_Sys = table.Column<double>(nullable: false),
                    SysCurrency = table.Column<string>(nullable: true),
                    TaxRate = table.Column<double>(nullable: false),
                    TaxValue = table.Column<double>(nullable: false),
                    Tel1 = table.Column<string>(nullable: true),
                    Tel2 = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false),
                    Total_Sys = table.Column<double>(nullable: false),
                    TypeDis = table.Column<string>(nullable: true),
                    UomName = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    VendorName = table.Column<string>(nullable: true),
                    VendorNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintPurchaseAP", x => x.Brand);
                });
        }
    }
}
