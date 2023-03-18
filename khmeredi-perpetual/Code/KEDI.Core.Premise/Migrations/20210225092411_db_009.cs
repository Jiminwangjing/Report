using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_009 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbDocumentType",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbDocumentType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbDocuNumbering",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Document = table.Column<string>(nullable: true),
                    DefaultSeries = table.Column<string>(nullable: true),
                    firstNo = table.Column<string>(nullable: true),
                    NextNo = table.Column<string>(nullable: true),
                    LastNo = table.Column<string>(nullable: true),
                    DocuTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbDocuNumbering", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbJournalEntry",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SeriesID = table.Column<int>(nullable: false),
                    Number = table.Column<string>(nullable: false),
                    DouTypeID = table.Column<int>(nullable: false),
                    Creator = table.Column<int>(nullable: false),
                    TransNo = table.Column<string>(nullable: false),
                    PostingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DueDate = table.Column<DateTime>(type: "Date", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Remarks = table.Column<string>(nullable: false),
                    TotalDebit = table.Column<decimal>(nullable: false),
                    TotalCredit = table.Column<decimal>(nullable: false),
                    SSCID = table.Column<int>(nullable: false),
                    LLCID = table.Column<int>(nullable: false),
                    LocalSetRate = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbJournalEntry", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbJournalEntryDetail",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    JEID = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    TypeID = table.Column<int>(nullable: false),
                    Dedit = table.Column<decimal>(nullable: false),
                    Credit = table.Column<decimal>(nullable: false),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbJournalEntryDetail", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbPeriodIndicator",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPeriodIndicator", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbPotingPeriod",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PeriodCode = table.Column<string>(nullable: true),
                    PeriodName = table.Column<string>(nullable: true),
                    SubPeriod = table.Column<string>(nullable: true),
                    NoOfPeroid = table.Column<string>(nullable: true),
                    PeroidIndID = table.Column<int>(nullable: false),
                    Category = table.Column<string>(nullable: true),
                    PeroidStatus = table.Column<int>(nullable: false),
                    PostingDateFrom = table.Column<DateTime>(type: "Date", nullable: false),
                    PostingDateTo = table.Column<DateTime>(type: "Date", nullable: false),
                    DueDateFrom = table.Column<DateTime>(type: "Date", nullable: false),
                    DueDateTo = table.Column<DateTime>(type: "Date", nullable: false),
                    DocuDateFrom = table.Column<DateTime>(type: "Date", nullable: false),
                    DocuDateTo = table.Column<DateTime>(type: "Date", nullable: false),
                    StartOfFiscalYear = table.Column<DateTime>(type: "Date", nullable: false),
                    FiscalYear = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPotingPeriod", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbSeries",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    firstNo = table.Column<string>(nullable: true),
                    NextNo = table.Column<string>(nullable: true),
                    LastNo = table.Column<string>(nullable: true),
                    DocuNumberingID = table.Column<int>(nullable: false),
                    PeriodIndID = table.Column<int>(nullable: false),
                    Default = table.Column<bool>(nullable: false),
                    Lock = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSeries", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbSeriesDetail",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NumberingID = table.Column<int>(nullable: false),
                    Number = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSeriesDetail", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbDocumentType",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbDocuNumbering",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbJournalEntry",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbJournalEntryDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPeriodIndicator",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPotingPeriod",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbSeries",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbSeriesDetail",
                schema: "dbo");
        }
    }
}
