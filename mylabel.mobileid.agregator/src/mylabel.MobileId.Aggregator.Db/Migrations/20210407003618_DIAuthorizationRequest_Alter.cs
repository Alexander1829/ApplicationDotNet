using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace mylabel.MobileId.Aggregator.Db.Migrations
{
    public partial class DIAuthorizationRequest_Alter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeSetAt",
                table: "DIAuthorizationRequests");

            migrationBuilder.DropColumn(
                name: "DcidSetAt",
                table: "DIAuthorizationRequests");

            migrationBuilder.DropColumn(
                name: "TokenEndpoint",
                table: "DIAuthorizationRequests");

            migrationBuilder.DropColumn(
                name: "TokenHashSetAt",
                table: "DIAuthorizationRequests");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CodeSetAt",
                table: "DIAuthorizationRequests",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DcidSetAt",
                table: "DIAuthorizationRequests",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenEndpoint",
                table: "DIAuthorizationRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "TokenHashSetAt",
                table: "DIAuthorizationRequests",
                type: "datetimeoffset",
                nullable: true);
        }
    }
}
