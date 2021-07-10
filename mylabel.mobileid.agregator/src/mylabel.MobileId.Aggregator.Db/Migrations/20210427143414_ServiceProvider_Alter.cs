using Microsoft.EntityFrameworkCore.Migrations;

namespace mylabel.MobileId.Aggregator.Db.Migrations
{
    public partial class ServiceProvider_Alter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientSecret",
                table: "DIAuthorizationRequests");

            migrationBuilder.AddColumn<bool>(
                name: "IsInactive",
                table: "ServiceProviders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPremiumInfoSigned",
                table: "ServiceProviders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ClientIdOnAggregator",
                table: "PremiumInfoTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInactive",
                table: "ServiceProviders");

            migrationBuilder.DropColumn(
                name: "IsPremiumInfoSigned",
                table: "ServiceProviders");

            migrationBuilder.DropColumn(
                name: "ClientIdOnAggregator",
                table: "PremiumInfoTokens");

            migrationBuilder.AddColumn<string>(
                name: "ClientSecret",
                table: "DIAuthorizationRequests",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
