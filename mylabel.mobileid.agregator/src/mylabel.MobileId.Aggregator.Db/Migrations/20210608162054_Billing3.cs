using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace mylabel.MobileId.Aggregator.Db.Migrations
{
    public partial class Billing3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BillingSuccess",
                table: "SIAuthorizationRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BillingSuccess",
                table: "DIAuthorizationRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BillingTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    record_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    subscriber_no = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    channel_seizure_date_time = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    message_switch_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    at_feature_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    call_action_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    call_to_tn_sgsn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Calling_no_ggsn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    guide_by = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    data_volume = table.Column<int>(type: "int", nullable: false),
                    call_source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    basic_service_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    basic_service_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    uom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    technology = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    call_destination = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiOrDi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorizationId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingTransactions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillingTransactions");

            migrationBuilder.DropColumn(
                name: "BillingSuccess",
                table: "SIAuthorizationRequests");

            migrationBuilder.DropColumn(
                name: "BillingSuccess",
                table: "DIAuthorizationRequests");
        }
    }
}
