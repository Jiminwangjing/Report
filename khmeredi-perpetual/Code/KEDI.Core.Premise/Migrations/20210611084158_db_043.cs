using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class db_043 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Value",
                schema: "dbo",
                table: "tbPropertyDetails",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "tbProperty",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "dbo",
                table: "tbProperty",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "dbo",
                table: "tbEmployee",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CostOfAccountingTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CACodeType = table.Column<string>(nullable: true),
                    CACodeName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostOfAccountingTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CostOfCenter",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ParentID = table.Column<int>(nullable: false),
                    MainParentID = table.Column<int>(nullable: false),
                    OwnerEmpID = table.Column<int>(nullable: false),
                    CostOfAccountingTypeID = table.Column<int>(nullable: true),
                    CompanyID = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    OwnerName = table.Column<string>(nullable: true),
                    CostOfAccountingType = table.Column<string>(nullable: true),
                    CostCenter = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ShortCode = table.Column<string>(nullable: true),
                    EffectiveFrom = table.Column<DateTime>(nullable: true),
                    EffectiveTo = table.Column<DateTime>(nullable: true),
                    ActiveDimension = table.Column<bool>(nullable: false),
                    IsDimension = table.Column<bool>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    None = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostOfCenter", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Dimensions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DimensionName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dimensions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbChildProperty",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbChildProperty", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CostOfAccountingTypes");

            migrationBuilder.DropTable(
                name: "CostOfCenter");

            migrationBuilder.DropTable(
                name: "Dimensions");

            migrationBuilder.DropTable(
                name: "tbChildProperty",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "tbProperty");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "dbo",
                table: "tbEmployee");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                schema: "dbo",
                table: "tbPropertyDetails",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "tbProperty",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
