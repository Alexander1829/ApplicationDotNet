using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace mylabel.MobileId.Aggregator.Db.Migrations
{
    public partial class PremiumInfoTokens_Alter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccessTokenHashOnAggregator",
                table: "PremiumInfoTokens",
                newName: "AccessTokenOnAggregatorHash");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "PremiumInfoTokens",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PremiumInfoTokens");

            migrationBuilder.RenameColumn(
                name: "AccessTokenOnAggregatorHash",
                table: "PremiumInfoTokens",
                newName: "AccessTokenHashOnAggregator");
        }
    }
}
