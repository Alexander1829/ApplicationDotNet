using Microsoft.EntityFrameworkCore.Migrations;

namespace mylabel.MobileId.Aggregator.Db.Migrations
{
    public partial class Billing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TokenHash",
                table: "DIAuthorizationRequests",
                newName: "AccessTokenOnAggregatorHash");

            migrationBuilder.AddColumn<string>(
                name: "AccessTokenOnAggregatorHash",
                table: "SIAuthorizationRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingCtn",
                table: "ServiceProviders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessTokenOnAggregatorHash",
                table: "SIAuthorizationRequests");

            migrationBuilder.DropColumn(
                name: "BillingCtn",
                table: "ServiceProviders");

            migrationBuilder.RenameColumn(
                name: "AccessTokenOnAggregatorHash",
                table: "DIAuthorizationRequests",
                newName: "TokenHash");
        }
    }
}
