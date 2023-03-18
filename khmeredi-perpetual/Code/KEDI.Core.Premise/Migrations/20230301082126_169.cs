using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _169 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupLoanPartners",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Grouploan = table.Column<int>(nullable: false),
                    Group1ID = table.Column<int>(nullable: false),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupLoanPartners", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LoanPartners",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Name1 = table.Column<string>(nullable: true),
                    Name2 = table.Column<string>(nullable: true),
                    Group1ID = table.Column<int>(nullable: false),
                    Group2ID = table.Column<int>(nullable: false),
                    EmpID = table.Column<int>(nullable: false),
                    EmloyeeName = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    VatNumber = table.Column<string>(nullable: true),
                    GPSLink = table.Column<string>(nullable: true),
                    RowId = table.Column<Guid>(nullable: false),
                    Spk = table.Column<int>(nullable: false),
                    Cpk = table.Column<int>(nullable: false),
                    Synced = table.Column<bool>(nullable: false),
                    SyncDate = table.Column<DateTime>(nullable: false),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanPartners", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LoanContactPersons",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LoanPartnerID = table.Column<int>(nullable: false),
                    ContactID = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    MidelName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Position = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Tel1 = table.Column<string>(nullable: true),
                    Tel2 = table.Column<string>(nullable: true),
                    MobilePhone = table.Column<string>(nullable: true),
                    Fax = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Pager = table.Column<string>(nullable: true),
                    Remark1 = table.Column<string>(nullable: true),
                    Remark2 = table.Column<string>(nullable: true),
                    Parssword = table.Column<string>(nullable: true),
                    ContryOfBirth = table.Column<int>(nullable: false),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    Profession = table.Column<string>(nullable: true),
                    SetAsDefualt = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanContactPersons", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LoanContactPersons_LoanPartners_LoanPartnerID",
                        column: x => x.LoanPartnerID,
                        principalTable: "LoanPartners",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanContactPersons_LoanPartnerID",
                table: "LoanContactPersons",
                column: "LoanPartnerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupLoanPartners");

            migrationBuilder.DropTable(
                name: "LoanContactPersons");

            migrationBuilder.DropTable(
                name: "LoanPartners");
        }
    }
}
