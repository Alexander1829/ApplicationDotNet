using Microsoft.EntityFrameworkCore.Migrations;

namespace mylabel.MobileId.Aggregator.Db.Migrations
{
    public partial class ServiceProviders_AllowedIPAddresses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AllowedIPAddresses",
                table: "ServiceProviders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedIPAddresses",
                table: "ServiceProviders");
        }
    }
}
