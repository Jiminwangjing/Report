using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _97 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InterestLevel",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestLevel", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "OpportunityMasterData",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Employee = table.Column<string>(nullable: true),
                    BPCode = table.Column<string>(nullable: true),
                    BPName = table.Column<string>(nullable: true),
                    Owner = table.Column<string>(nullable: true),
                    OpportunityName = table.Column<string>(nullable: true),
                    OpportunityNo = table.Column<int>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    ClosingDate = table.Column<DateTime>(nullable: false),
                    CloingPercentage = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunityMasterData", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ProjectCostAnalyses",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    CusID = table.Column<int>(nullable: false),
                    CusContactID = table.Column<int>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    SeriesDID = table.Column<int>(nullable: false),
                    SeriesNo = table.Column<string>(nullable: true),
                    WarehouseID = table.Column<int>(nullable: false),
                    CustomerRef = table.Column<string>(nullable: true),
                    PriceListID = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    PostingDate = table.Column<DateTime>(nullable: false),
                    ValidUntilDate = table.Column<DateTime>(nullable: false),
                    Documentdate = table.Column<DateTime>(nullable: false),
                    Barcodereadign = table.Column<string>(nullable: true),
                    SaleEmID = table.Column<int>(nullable: false),
                    OwnerID = table.Column<int>(nullable: false),
                    ExchangeRate = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    SubTotalBeforeDis = table.Column<decimal>(nullable: false),
                    Discount = table.Column<decimal>(nullable: false),
                    SubTotalAfterDis = table.Column<decimal>(nullable: false),
                    FreightAmount = table.Column<decimal>(nullable: false),
                    Tax = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    TotalMargin = table.Column<decimal>(nullable: false),
                    TotalCommission = table.Column<decimal>(nullable: false),
                    OtherCost = table.Column<decimal>(nullable: false),
                    ExpectedTotalProfit = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectCostAnalyses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SaleEmployee",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Action = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleEmployee", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SetupCompetitor",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    ThreaLevelID = table.Column<int>(nullable: false),
                    Detail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetupCompetitor", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SetupInterestRange",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DescriptionLevel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetupInterestRange", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SetupPartner",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    RelatedBp = table.Column<string>(nullable: true),
                    DFRelationship = table.Column<int>(nullable: false),
                    Detail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetupPartner", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SetupReasons",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetupReasons", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SetupRelationship",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RelationshipDscription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetupRelationship", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SetUpStage",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    ClosingPercentTage = table.Column<float>(nullable: false),
                    StageNo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetUpStage", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CompetitorDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OpportunityMasterDataID = table.Column<int>(nullable: false),
                    NameCompetitorID = table.Column<int>(nullable: false),
                    ThrealevelID = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitorDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CompetitorDetail_OpportunityMasterData_OpportunityMasterDataID",
                        column: x => x.OpportunityMasterDataID,
                        principalTable: "OpportunityMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartnerDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OpportunityMasterDataID = table.Column<int>(nullable: false),
                    NamePartnerID = table.Column<int>(nullable: false),
                    RelationshipID = table.Column<int>(nullable: false),
                    RelatedBp = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PartnerDetail_OpportunityMasterData_OpportunityMasterDataID",
                        column: x => x.OpportunityMasterDataID,
                        principalTable: "OpportunityMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PotentialDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OpportunityMasterDataID = table.Column<int>(nullable: false),
                    PredictedClosingInTime = table.Column<int>(nullable: false),
                    PredictedClosingInNum = table.Column<int>(nullable: false),
                    PredictedClosingDate = table.Column<DateTime>(nullable: false),
                    PotentailAmount = table.Column<decimal>(nullable: false),
                    WeightAmount = table.Column<decimal>(nullable: false),
                    GrossProfit = table.Column<decimal>(nullable: false),
                    GrossProfitTotal = table.Column<decimal>(nullable: false),
                    Level = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PotentialDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PotentialDetail_OpportunityMasterData_OpportunityMasterDataID",
                        column: x => x.OpportunityMasterDataID,
                        principalTable: "OpportunityMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StageDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StartDate = table.Column<DateTime>(nullable: false),
                    CloseDate = table.Column<DateTime>(nullable: false),
                    OpportunityMasterDataID = table.Column<int>(nullable: false),
                    SaleEmpselectID = table.Column<int>(nullable: false),
                    StagesID = table.Column<int>(nullable: false),
                    Percent = table.Column<float>(nullable: false),
                    PotentailAmount = table.Column<decimal>(nullable: false),
                    WeightAmount = table.Column<decimal>(nullable: false),
                    ShowBpsDoc = table.Column<bool>(nullable: false),
                    DocTypeID = table.Column<int>(nullable: false),
                    DocNo = table.Column<int>(nullable: false),
                    OwnerID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_StageDetail_OpportunityMasterData_OpportunityMasterDataID",
                        column: x => x.OpportunityMasterDataID,
                        principalTable: "OpportunityMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SummaryDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OpportunityMasterDataID = table.Column<int>(nullable: false),
                    DocTypeID = table.Column<int>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    SeriesDID = table.Column<int>(nullable: false),
                    IsOpen = table.Column<bool>(nullable: false),
                    IsLost = table.Column<bool>(nullable: false),
                    IsWon = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummaryDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SummaryDetail_OpportunityMasterData_OpportunityMasterDataID",
                        column: x => x.OpportunityMasterDataID,
                        principalTable: "OpportunityMasterData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FreightProjCostDetails",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectCostAnalysisID = table.Column<int>(nullable: false),
                    TaxGroupID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TaxRate = table.Column<decimal>(nullable: false),
                    TotalTaxAmount = table.Column<decimal>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    AmountWithTax = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreightProjCostDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FreightProjCostDetails_ProjectCostAnalyses_ProjectCostAnalysisID",
                        column: x => x.ProjectCostAnalysisID,
                        principalTable: "ProjectCostAnalyses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjCostAnalysisDetails",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectCostAnalysisID = table.Column<int>(nullable: false),
                    ItemMaterDataID = table.Column<int>(nullable: false),
                    ItemCode = table.Column<string>(nullable: true),
                    Barcode = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(nullable: false),
                    UomID = table.Column<int>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    Cost = table.Column<decimal>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false),
                    DisRate = table.Column<decimal>(nullable: false),
                    DisValue = table.Column<decimal>(nullable: false),
                    UnitPriceAfterDis = table.Column<decimal>(nullable: false),
                    LineTotalBeforeDis = table.Column<decimal>(nullable: false),
                    Total = table.Column<decimal>(nullable: false),
                    LineTotalCost = table.Column<decimal>(nullable: false),
                    TaxGroupID = table.Column<int>(nullable: false),
                    TaxRate = table.Column<decimal>(nullable: false),
                    TaxValue = table.Column<decimal>(nullable: false),
                    TaxOfFinDisValue = table.Column<decimal>(nullable: false),
                    FinDisRate = table.Column<decimal>(nullable: false),
                    FinDisValue = table.Column<decimal>(nullable: false),
                    UnitMargin = table.Column<decimal>(nullable: false),
                    TotalWTax = table.Column<decimal>(nullable: false),
                    LineTotalMargin = table.Column<decimal>(nullable: false),
                    InStock = table.Column<double>(nullable: false),
                    FinTotalValue = table.Column<decimal>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjCostAnalysisDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProjCostAnalysisDetails_ProjectCostAnalyses_ProjectCostAnalysisID",
                        column: x => x.ProjectCostAnalysisID,
                        principalTable: "ProjectCostAnalyses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DescriptionPotentialDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    interestID = table.Column<int>(nullable: false),
                    PotentialDetailID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DescriptionPotentialDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DescriptionPotentialDetail_PotentialDetail_PotentialDetailID",
                        column: x => x.PotentialDetailID,
                        principalTable: "PotentialDetail",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DescriptionSummaryDetail",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReasonsID = table.Column<int>(nullable: false),
                    SummaryDetailID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DescriptionSummaryDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DescriptionSummaryDetail_SummaryDetail_SummaryDetailID",
                        column: x => x.SummaryDetailID,
                        principalTable: "SummaryDetail",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompetitorDetail_OpportunityMasterDataID",
                table: "CompetitorDetail",
                column: "OpportunityMasterDataID");

            migrationBuilder.CreateIndex(
                name: "IX_DescriptionPotentialDetail_PotentialDetailID",
                table: "DescriptionPotentialDetail",
                column: "PotentialDetailID");

            migrationBuilder.CreateIndex(
                name: "IX_DescriptionSummaryDetail_SummaryDetailID",
                table: "DescriptionSummaryDetail",
                column: "SummaryDetailID");

            migrationBuilder.CreateIndex(
                name: "IX_FreightProjCostDetails_ProjectCostAnalysisID",
                table: "FreightProjCostDetails",
                column: "ProjectCostAnalysisID");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerDetail_OpportunityMasterDataID",
                table: "PartnerDetail",
                column: "OpportunityMasterDataID");

            migrationBuilder.CreateIndex(
                name: "IX_PotentialDetail_OpportunityMasterDataID",
                table: "PotentialDetail",
                column: "OpportunityMasterDataID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjCostAnalysisDetails_ProjectCostAnalysisID",
                table: "ProjCostAnalysisDetails",
                column: "ProjectCostAnalysisID");

            migrationBuilder.CreateIndex(
                name: "IX_StageDetail_OpportunityMasterDataID",
                table: "StageDetail",
                column: "OpportunityMasterDataID");

            migrationBuilder.CreateIndex(
                name: "IX_SummaryDetail_OpportunityMasterDataID",
                table: "SummaryDetail",
                column: "OpportunityMasterDataID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompetitorDetail");

            migrationBuilder.DropTable(
                name: "DescriptionPotentialDetail");

            migrationBuilder.DropTable(
                name: "DescriptionSummaryDetail");

            migrationBuilder.DropTable(
                name: "FreightProjCostDetails");

            migrationBuilder.DropTable(
                name: "InterestLevel");

            migrationBuilder.DropTable(
                name: "PartnerDetail");

            migrationBuilder.DropTable(
                name: "ProjCostAnalysisDetails");

            migrationBuilder.DropTable(
                name: "SaleEmployee");

            migrationBuilder.DropTable(
                name: "SetupCompetitor");

            migrationBuilder.DropTable(
                name: "SetupInterestRange");

            migrationBuilder.DropTable(
                name: "SetupPartner");

            migrationBuilder.DropTable(
                name: "SetupReasons");

            migrationBuilder.DropTable(
                name: "SetupRelationship");

            migrationBuilder.DropTable(
                name: "SetUpStage");

            migrationBuilder.DropTable(
                name: "StageDetail");

            migrationBuilder.DropTable(
                name: "PotentialDetail");

            migrationBuilder.DropTable(
                name: "SummaryDetail");

            migrationBuilder.DropTable(
                name: "ProjectCostAnalyses");

            migrationBuilder.DropTable(
                name: "OpportunityMasterData");
        }
    }
}
