using Microsoft.EntityFrameworkCore.Migrations;

namespace mylabel.MobileId.Aggregator.Db.Migrations
{
    public partial class PremiumInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PremiumInfoEndpoint",
                table: "DIAuthorizationRequests",
                newName: "ServingOperator");

            migrationBuilder.AddColumn<string>(
                name: "PremiumInfoEndpoint",
                table: "SIAuthorizationRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServingOperator",
                table: "SIAuthorizationRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdgwClientId",
                table: "DIAuthorizationRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdgwClientSecret",
                table: "DIAuthorizationRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PremiumInfoTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServingOperator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccessTokenOnIdgw = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccessTokenHashOnAggregator = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremiumInfoTokens", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PremiumInfoTokens");

            migrationBuilder.DropColumn(
                name: "PremiumInfoEndpoint",
                table: "SIAuthorizationRequests");

            migrationBuilder.DropColumn(
                name: "ServingOperator",
                table: "SIAuthorizationRequests");

            migrationBuilder.DropColumn(
                name: "IdgwClientId",
                table: "DIAuthorizationRequests");

            migrationBuilder.DropColumn(
                name: "IdgwClientSecret",
                table: "DIAuthorizationRequests");

            migrationBuilder.RenameColumn(
                name: "ServingOperator",
                table: "DIAuthorizationRequests",
                newName: "PremiumInfoEndpoint");
        }
    }
}
