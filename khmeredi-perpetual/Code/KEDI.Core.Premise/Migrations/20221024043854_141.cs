using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _141 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachmentFileOfContractTemplate_ContractTemplate_ContractTemplateID",
                table: "AttachmentFileOfContractTemplate");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractTemplate_Converage_ConverageID",
                table: "ContractTemplate");

            migrationBuilder.DropColumn(
                name: "ContractID",
                table: "Remark");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "General");

            migrationBuilder.DropColumn(
                name: "ContractID",
                table: "Converage");

            migrationBuilder.DropColumn(
                name: "ContractID",
                table: "AttachmentFileOfContractTemplate");

            migrationBuilder.RenameColumn(
                name: "OpexID",
                table: "tbGLAccount",
                newName: "SubTypeAccountID");

            migrationBuilder.AddColumn<double>(
                name: "ReturnQty",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "PaymentCode",
                schema: "dbo",
                table: "tbBusinessPartner",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryType",
                table: "tbGLAccount",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "SetupStatus",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusID",
                table: "General",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ConverageID",
                table: "ContractTemplate",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ContractTemplateID",
                table: "AttachmentFileOfContractTemplate",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssignByID",
                table: "Activity",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SubTypeAcounts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Default = table.Column<bool>(nullable: false),
                    Delete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubTypeAcounts", x => x.ID);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AttachmentFileOfContractTemplate_ContractTemplate_ContractTemplateID",
                table: "AttachmentFileOfContractTemplate",
                column: "ContractTemplateID",
                principalTable: "ContractTemplate",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractTemplate_Converage_ConverageID",
                table: "ContractTemplate",
                column: "ConverageID",
                principalTable: "Converage",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachmentFileOfContractTemplate_ContractTemplate_ContractTemplateID",
                table: "AttachmentFileOfContractTemplate");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractTemplate_Converage_ConverageID",
                table: "ContractTemplate");

            migrationBuilder.DropTable(
                name: "SubTypeAcounts");

            migrationBuilder.DropColumn(
                name: "ReturnQty",
                schema: "dbo",
                table: "tbOrderDetail");

            migrationBuilder.DropColumn(
                name: "PaymentCode",
                schema: "dbo",
                table: "tbBusinessPartner");

            migrationBuilder.DropColumn(
                name: "CategoryType",
                table: "tbGLAccount");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "SetupStatus");

            migrationBuilder.DropColumn(
                name: "StatusID",
                table: "General");

            migrationBuilder.DropColumn(
                name: "AssignByID",
                table: "Activity");

            migrationBuilder.RenameColumn(
                name: "SubTypeAccountID",
                table: "tbGLAccount",
                newName: "OpexID");

            migrationBuilder.AddColumn<int>(
                name: "ContractID",
                table: "Remark",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "General",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContractID",
                table: "Converage",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ConverageID",
                table: "ContractTemplate",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ContractTemplateID",
                table: "AttachmentFileOfContractTemplate",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "ContractID",
                table: "AttachmentFileOfContractTemplate",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_AttachmentFileOfContractTemplate_ContractTemplate_ContractTemplateID",
                table: "AttachmentFileOfContractTemplate",
                column: "ContractTemplateID",
                principalTable: "ContractTemplate",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractTemplate_Converage_ConverageID",
                table: "ContractTemplate",
                column: "ConverageID",
                principalTable: "Converage",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
