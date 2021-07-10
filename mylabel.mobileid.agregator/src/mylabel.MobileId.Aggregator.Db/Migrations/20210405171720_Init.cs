using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace mylabel.MobileId.Aggregator.Db.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DIAuthorizationRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateNew = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RedirectUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcrValues = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nonce = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Display = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Dcid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DcidSetAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TokenEndpoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodeSetAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    TokenHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenHashSetAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PremiumInfoEndpoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DIAuthorizationRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscoveryServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Uri = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscoveryServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientIdOnAggregator = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AggregatorClientSecretHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JwksUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JwksValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UseStoredJwksValue = table.Column<bool>(type: "bit", nullable: false),
                    JwksCachingInSeconds = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SIAuthorizationRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SPNotificationUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SPNotificationToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SPNonce = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientIdOnAggregator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdgwJwksUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorizationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AggregatorNotificationToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AggregatorNonce = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RespondedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIAuthorizationRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SPNotificationUris",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceProviderId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPNotificationUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SPNotificationUris_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SPRedirectUris",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceProviderId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPRedirectUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SPRedirectUris_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SPToDiscoveryLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceProviderId = table.Column<int>(type: "int", nullable: false),
                    DiscoveryServiceId = table.Column<int>(type: "int", nullable: false),
                    ClientIdOnDiscovery = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientSecretOnDiscovery = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RedirectUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPToDiscoveryLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SPToDiscoveryLinks_DiscoveryServices_DiscoveryServiceId",
                        column: x => x.DiscoveryServiceId,
                        principalTable: "DiscoveryServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SPToDiscoveryLinks_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviders_ClientIdOnAggregator",
                table: "ServiceProviders",
                column: "ClientIdOnAggregator",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIAuthorizationRequests_AuthorizationRequestId",
                table: "SIAuthorizationRequests",
                column: "AuthorizationRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SPNotificationUris_ServiceProviderId",
                table: "SPNotificationUris",
                column: "ServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_SPRedirectUris_ServiceProviderId",
                table: "SPRedirectUris",
                column: "ServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_SPToDiscoveryLinks_DiscoveryServiceId",
                table: "SPToDiscoveryLinks",
                column: "DiscoveryServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SPToDiscoveryLinks_ServiceProviderId_DiscoveryServiceId",
                table: "SPToDiscoveryLinks",
                columns: new[] { "ServiceProviderId", "DiscoveryServiceId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DIAuthorizationRequests");

            migrationBuilder.DropTable(
                name: "SIAuthorizationRequests");

            migrationBuilder.DropTable(
                name: "SPNotificationUris");

            migrationBuilder.DropTable(
                name: "SPRedirectUris");

            migrationBuilder.DropTable(
                name: "SPToDiscoveryLinks");

            migrationBuilder.DropTable(
                name: "DiscoveryServices");

            migrationBuilder.DropTable(
                name: "ServiceProviders");
        }
    }
}
