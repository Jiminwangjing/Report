﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace CKBS.Migrations
{
    public partial class _64 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupUomID",
                schema: "dbo",
                table: "tbOrderDetail",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupUomID",
                schema: "dbo",
                table: "tbOrderDetail");
        }
    }
}
