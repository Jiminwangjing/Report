using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _110 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BPCode",
                table: "OpportunityMasterData");

            migrationBuilder.DropColumn(
                name: "BPName",
                table: "OpportunityMasterData");

            migrationBuilder.DropColumn(
                name: "Employee",
                table: "OpportunityMasterData");

            migrationBuilder.AddColumn<decimal>(
                name: "CreditLimit",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "GPSink",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name2",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentTermsID",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ActivityID",
                table: "StageDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BPRefNo",
                table: "ServiceCalls",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CallID",
                table: "ServiceCalls",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChannelID",
                table: "ServiceCalls",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HandledByID",
                table: "ServiceCalls",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Resolution",
                table: "ServiceCalls",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TechnicianID",
                table: "ServiceCalls",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BPID",
                table: "OpportunityMasterData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaleEmpID",
                table: "OpportunityMasterData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Activity",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<int>(nullable: false),
                    EmpID = table.Column<int>(nullable: false),
                    Activities = table.Column<int>(nullable: false),
                    EmpName = table.Column<string>(nullable: true),
                    SubName = table.Column<string>(nullable: true),
                    Employee = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Number = table.Column<int>(nullable: false),
                    BPID = table.Column<int>(nullable: false),
                    Personal = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activity", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BPBranch",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusinessPartnerID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Tel = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    BranchCotactPerson = table.Column<string>(nullable: true),
                    ContactTel = table.Column<string>(nullable: true),
                    ContactEmail = table.Column<string>(nullable: true),
                    GPSLink = table.Column<string>(nullable: true),
                    SetDefualt = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BPBranch", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BPBranch_tbBusinessPartner_BusinessPartnerID",
                        column: x => x.BusinessPartnerID,
                        principalSchema: "dbo",
                        principalTable: "tbBusinessPartner",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ContactPerson",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusinessPartnerID = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_ContactPerson", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ContactPerson_tbBusinessPartner_BusinessPartnerID",
                        column: x => x.BusinessPartnerID,
                        principalSchema: "dbo",
                        principalTable: "tbBusinessPartner",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(nullable: true),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SetupActivity",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Activities = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetupActivity", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SetupLocation",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetupLocation", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SetupStatus",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetupStatus", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SetupSubject",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    TypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetupSubject", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SetupType",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetupType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbCashDicount",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodeName = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ByDate = table.Column<bool>(nullable: false),
                    Freight = table.Column<bool>(nullable: false),
                    Day = table.Column<int>(nullable: false),
                    Month = table.Column<int>(nullable: false),
                    Discount = table.Column<float>(nullable: false),
                    CashDiscountDay = table.Column<float>(nullable: false),
                    DiscountPercent = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbCashDicount", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbInstaillments",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NoOfInstaillment = table.Column<int>(nullable: false),
                    ApplyTax = table.Column<bool>(nullable: false),
                    UpdateTax = table.Column<bool>(nullable: false),
                    CreditMethod = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbInstaillments", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbPaymentTerms",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Months = table.Column<int>(nullable: false),
                    Days = table.Column<int>(nullable: false),
                    StartFrom = table.Column<int>(nullable: true),
                    DueDate = table.Column<int>(nullable: true),
                    OpenIncomingPayment = table.Column<int>(nullable: true),
                    TolerenceDay = table.Column<string>(nullable: true),
                    TotalDiscount = table.Column<float>(nullable: false),
                    InterestOnReceiVables = table.Column<float>(nullable: false),
                    MaxCredit = table.Column<float>(nullable: false),
                    CommitLimit = table.Column<float>(nullable: false),
                    PriceListID = table.Column<int>(nullable: false),
                    InstaillmentID = table.Column<int>(nullable: false),
                    CashDiscountID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbPaymentTerms", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "General",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Remark = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    Durration = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Recurrence = table.Column<string>(nullable: true),
                    Priority = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    ActivityID = table.Column<int>(nullable: false),
                    RepeatDate = table.Column<int>(nullable: false),
                    RepeatEveryRecurr = table.Column<bool>(nullable: false),
                    RepeatEveryWeek = table.Column<bool>(nullable: false),
                    Mon = table.Column<bool>(nullable: false),
                    Tue = table.Column<bool>(nullable: false),
                    Wed = table.Column<bool>(nullable: false),
                    Thu = table.Column<bool>(nullable: false),
                    Fri = table.Column<bool>(nullable: false),
                    Sat = table.Column<bool>(nullable: false),
                    Sun = table.Column<bool>(nullable: false),
                    RepeatNumOfmonths = table.Column<int>(nullable: false),
                    Days = table.Column<bool>(nullable: false),
                    numDay = table.Column<int>(nullable: false),
                    repeatOn = table.Column<bool>(nullable: false),
                    numOfRepeat = table.Column<string>(nullable: true),
                    DaysInMonthly = table.Column<string>(nullable: true),
                    RepeatofNumAnnualy = table.Column<int>(nullable: false),
                    RepeatOncheckYearly = table.Column<bool>(nullable: false),
                    MonthsInAnnualy = table.Column<string>(nullable: true),
                    NumOfMonths = table.Column<int>(nullable: false),
                    checkNumAnnualy = table.Column<bool>(nullable: false),
                    NumofAnnualy = table.Column<string>(nullable: true),
                    DaysOfAnnualy = table.Column<string>(nullable: true),
                    MonthsOfAnnulay = table.Column<string>(nullable: true),
                    Start = table.Column<DateTime>(nullable: false),
                    NoEndDate = table.Column<bool>(nullable: false),
                    After = table.Column<bool>(nullable: false),
                    NumAfter = table.Column<int>(nullable: false),
                    By = table.Column<bool>(nullable: false),
                    ByDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_General", x => x.ID);
                    table.ForeignKey(
                        name: "FK_General_Activity_ActivityID",
                        column: x => x.ActivityID,
                        principalTable: "Activity",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbInstallmentDetail",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InstaillmentID = table.Column<int>(nullable: false),
                    Months = table.Column<int>(nullable: false),
                    Day = table.Column<int>(nullable: false),
                    Percent = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbInstallmentDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tbInstallmentDetail_tbInstaillments_InstaillmentID",
                        column: x => x.InstaillmentID,
                        principalSchema: "dbo",
                        principalTable: "tbInstaillments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BPBranch_BusinessPartnerID",
                table: "BPBranch",
                column: "BusinessPartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_ContactPerson_BusinessPartnerID",
                table: "ContactPerson",
                column: "BusinessPartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_General_ActivityID",
                table: "General",
                column: "ActivityID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbInstallmentDetail_InstaillmentID",
                schema: "dbo",
                table: "tbInstallmentDetail",
                column: "InstaillmentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BPBranch");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "ContactPerson");

            migrationBuilder.DropTable(
                name: "EmployeeTypes");

            migrationBuilder.DropTable(
                name: "General");

            migrationBuilder.DropTable(
                name: "SetupActivity");

            migrationBuilder.DropTable(
                name: "SetupLocation");

            migrationBuilder.DropTable(
                name: "SetupStatus");

            migrationBuilder.DropTable(
                name: "SetupSubject");

            migrationBuilder.DropTable(
                name: "SetupType");

            migrationBuilder.DropTable(
                name: "tbCashDicount",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbInstallmentDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tbPaymentTerms",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Activity");

            migrationBuilder.DropTable(
                name: "tbInstaillments",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "CreditLimit",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "GPSink",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "Name2",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "PaymentTermsID",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "ActivityID",
                table: "StageDetail");

            migrationBuilder.DropColumn(
                name: "BPRefNo",
                table: "ServiceCalls");

            migrationBuilder.DropColumn(
                name: "CallID",
                table: "ServiceCalls");

            migrationBuilder.DropColumn(
                name: "ChannelID",
                table: "ServiceCalls");

            migrationBuilder.DropColumn(
                name: "HandledByID",
                table: "ServiceCalls");

            migrationBuilder.DropColumn(
                name: "Resolution",
                table: "ServiceCalls");

            migrationBuilder.DropColumn(
                name: "TechnicianID",
                table: "ServiceCalls");

            migrationBuilder.DropColumn(
                name: "BPID",
                table: "OpportunityMasterData");

            migrationBuilder.DropColumn(
                name: "SaleEmpID",
                table: "OpportunityMasterData");

            migrationBuilder.AddColumn<string>(
                name: "BPCode",
                table: "OpportunityMasterData",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BPName",
                table: "OpportunityMasterData",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Employee",
                table: "OpportunityMasterData",
                nullable: true);
        }
    }
}
