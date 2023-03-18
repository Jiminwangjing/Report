using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _120 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BaseOnID",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CopyType",
                table: "tbProjectCostAnalysis",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "KeyCopy",
                table: "tbProjectCostAnalysis",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OpexID",
                table: "tbGLAccount",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "tbOpexs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbOpexs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbServiceData",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServiceCallID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    MfrSerialNo = table.Column<string>(nullable: true),
                    SerialNo = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbServiceData", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbServiceData_ServiceCalls_ServiceCallID",
                        column: x => x.ServiceCallID,
                        principalTable: "ServiceCalls",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbSolutionDataManagement",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    CusID = table.Column<int>(nullable: false),
                    ConTactID = table.Column<int>(nullable: false),
                    BranchID = table.Column<int>(nullable: false),
                    WarehouseID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    SaleCurrencyID = table.Column<int>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    DocTypeID = table.Column<int>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    SeriesDID = table.Column<int>(nullable: false),
                    InvoiceNumber = table.Column<string>(nullable: true),
                    RefNo = table.Column<string>(nullable: true),
                    InvoiceNo = table.Column<string>(nullable: true),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    ValidUntilDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    SaleEMID = table.Column<int>(nullable: false),
                    OwnerID = table.Column<int>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSolutionDataManagement", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbServiceItem",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServiceDataID = table.Column<int>(nullable: false),
                    ActivityID = table.Column<int>(nullable: false),
                    HandledByID = table.Column<int>(nullable: false),
                    TechnicianID = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    StartTime = table.Column<TimeSpan>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<TimeSpan>(nullable: false),
                    Completed = table.Column<double>(nullable: false),
                    Finnish = table.Column<bool>(nullable: false),
                    Acitivity = table.Column<bool>(nullable: false),
                    LinkActivytyID = table.Column<int>(nullable: false),
                    ActivityName = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbServiceItem", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbServiceItem_tbServiceData_ServiceDataID",
                        column: x => x.ServiceDataID,
                        principalTable: "tbServiceData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbSolutionDataManagementDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SolutionDataManagementID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    ItemCode = table.Column<string>(nullable: true),
                    ItemNameKH = table.Column<string>(nullable: true),
                    ItemNameEN = table.Column<string>(nullable: true),
                    Qty = table.Column<double>(nullable: false),
                    GUomID = table.Column<int>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    UomName = table.Column<string>(nullable: true),
                    InStock = table.Column<double>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSolutionDataManagementDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbSolutionDataManagementDetail_tbSolutionDataManagement_SolutionDataManagementID",
                        column: x => x.SolutionDataManagementID,
                        principalTable: "tbSolutionDataManagement",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbServiceData_ServiceCallID",
                table: "tbServiceData",
                column: "ServiceCallID");

            migrationBuilder.CreateIndex(
                name: "IX_tbServiceItem_ServiceDataID",
                table: "tbServiceItem",
                column: "ServiceDataID");

            migrationBuilder.CreateIndex(
                name: "IX_tbSolutionDataManagementDetail_SolutionDataManagementID",
                table: "tbSolutionDataManagementDetail",
                column: "SolutionDataManagementID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbOpexs");

            migrationBuilder.DropTable(
                name: "tbServiceItem");

            migrationBuilder.DropTable(
                name: "tbSolutionDataManagementDetail");

            migrationBuilder.DropTable(
                name: "tbServiceData");

            migrationBuilder.DropTable(
                name: "tbSolutionDataManagement");

            migrationBuilder.DropColumn(
                name: "BaseOnID",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "CopyType",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "KeyCopy",
                table: "tbProjectCostAnalysis");

            migrationBuilder.DropColumn(
                name: "OpexID",
                table: "tbGLAccount");
        }
    }
}
