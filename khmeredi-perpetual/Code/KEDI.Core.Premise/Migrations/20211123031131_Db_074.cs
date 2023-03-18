using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class Db_074 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractID",
                schema: "dbo",
                table: "tbItemMasterData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaxGroupPurID",
                schema: "dbo",
                table: "tbItemMasterData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Contract",
                table: "StockOut",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ResponseTimeValue = table.Column<decimal>(nullable: false),
                    ResponseTimeType = table.Column<int>(nullable: false),
                    ResolutionTimeValue = table.Column<decimal>(nullable: false),
                    ResolutionTimeType = table.Column<int>(nullable: false),
                    Expired = table.Column<bool>(nullable: false),
                    Duration = table.Column<decimal>(nullable: false),
                    Renewal = table.Column<bool>(nullable: false),
                    ReminderType = table.Column<int>(nullable: false),
                    ContractType = table.Column<int>(nullable: false),
                    ReminderValue = table.Column<decimal>(nullable: false),
                    Monday = table.Column<bool>(nullable: false),
                    Tuesday = table.Column<bool>(nullable: false),
                    Wednesday = table.Column<bool>(nullable: false),
                    Thursday = table.Column<bool>(nullable: false),
                    Friday = table.Column<bool>(nullable: false),
                    Saturday = table.Column<bool>(nullable: false),
                    Sunday = table.Column<bool>(nullable: false),
                    MondayStartTime = table.Column<string>(nullable: true),
                    TuesdayStartTime = table.Column<string>(nullable: true),
                    WednesdayStartTime = table.Column<string>(nullable: true),
                    ThursdayStartTime = table.Column<string>(nullable: true),
                    FridayStartTime = table.Column<string>(nullable: true),
                    SaturdayStartTime = table.Column<string>(nullable: true),
                    SundayStartTime = table.Column<string>(nullable: true),
                    MondayEndTime = table.Column<string>(nullable: true),
                    TuesdayEndTime = table.Column<string>(nullable: true),
                    WednesdayEndTime = table.Column<string>(nullable: true),
                    ThursdayEndTime = table.Column<string>(nullable: true),
                    FridayEndTime = table.Column<string>(nullable: true),
                    SaturdayEndTime = table.Column<string>(nullable: true),
                    SundayEndTime = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    Parts = table.Column<bool>(nullable: false),
                    Labor = table.Column<bool>(nullable: false),
                    Travel = table.Column<bool>(nullable: false),
                    Includeholiday = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCalls",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    BPID = table.Column<int>(nullable: false),
                    MfrSerialNo = table.Column<string>(nullable: true),
                    SerialNumber = table.Column<string>(nullable: true),
                    ItemID = table.Column<int>(nullable: false),
                    ItemGroupID = table.Column<int>(nullable: false),
                    SeriesID = table.Column<int>(nullable: false),
                    SeriesDID = table.Column<int>(nullable: false),
                    DocTypeID = table.Column<int>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    CallStatus = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    CreatedOnDate = table.Column<DateTime>(nullable: false),
                    CreatedOnTime = table.Column<string>(nullable: true),
                    ClosedOnDate = table.Column<DateTime>(nullable: true),
                    ClosedOnTime = table.Column<string>(nullable: true),
                    ContractNo = table.Column<string>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Subject = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCalls", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "ServiceCalls");

            migrationBuilder.DropColumn(
                name: "ContractID",
                schema: "dbo",
                table: "tbItemMasterData");

            migrationBuilder.DropColumn(
                name: "TaxGroupPurID",
                schema: "dbo",
                table: "tbItemMasterData");

            migrationBuilder.DropColumn(
                name: "Contract",
                table: "StockOut");
        }
    }
}
