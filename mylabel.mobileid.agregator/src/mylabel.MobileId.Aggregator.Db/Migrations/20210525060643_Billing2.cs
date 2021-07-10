using Microsoft.EntityFrameworkCore.Migrations;

namespace mylabel.MobileId.Aggregator.Db.Migrations
{
    public partial class Billing2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientIdOnAggregator",
                table: "PremiumInfoTokens");

            migrationBuilder.RenameColumn(
                name: "ServingOperator",
                table: "PremiumInfoTokens",
                newName: "AfterSiOrDi");

            migrationBuilder.AddColumn<string>(
                name: "AcrValues",
                table: "SIAuthorizationRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Msisdn",
                table: "SIAuthorizationRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Scope",
                table: "SIAuthorizationRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Msisdn",
                table: "DIAuthorizationRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcrValues",
                table: "SIAuthorizationRequests");

            migrationBuilder.DropColumn(
                name: "Msisdn",
                table: "SIAuthorizationRequests");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "SIAuthorizationRequests");

            migrationBuilder.DropColumn(
                name: "Msisdn",
                table: "DIAuthorizationRequests");

            migrationBuilder.RenameColumn(
                name: "AfterSiOrDi",
                table: "PremiumInfoTokens",
                newName: "ServingOperator");

            migrationBuilder.AddColumn<string>(
                name: "ClientIdOnAggregator",
                table: "PremiumInfoTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
