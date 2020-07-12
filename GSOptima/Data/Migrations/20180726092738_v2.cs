using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GSOptima.Data.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MembershipType",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    StockID = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock", x => x.StockID);
                });

            migrationBuilder.CreateTable(
                name: "StockPrice",
                columns: table => new
                {
                    StockID = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Action = table.Column<string>(nullable: true),
                    AverageBigWave = table.Column<decimal>(nullable: true),
                    BBLower = table.Column<decimal>(nullable: true),
                    BBUpper = table.Column<decimal>(nullable: true),
                    BigWave = table.Column<decimal>(nullable: true),
                    Close = table.Column<decimal>(nullable: false),
                    EMA12 = table.Column<decimal>(nullable: true),
                    EMA26 = table.Column<decimal>(nullable: true),
                    Frequency = table.Column<long>(nullable: true),
                    GSLine = table.Column<decimal>(nullable: true),
                    GSLineDirection = table.Column<string>(nullable: true),
                    High = table.Column<decimal>(nullable: false),
                    Highest12Months = table.Column<decimal>(nullable: true),
                    Highest3Months = table.Column<decimal>(nullable: true),
                    Highest6Months = table.Column<decimal>(nullable: true),
                    Low = table.Column<decimal>(nullable: false),
                    Lowest12Months = table.Column<decimal>(nullable: true),
                    Lowest3Months = table.Column<decimal>(nullable: true),
                    Lowest6Months = table.Column<decimal>(nullable: true),
                    MA20 = table.Column<decimal>(nullable: true),
                    MA60 = table.Column<decimal>(nullable: true),
                    MACD = table.Column<decimal>(nullable: true),
                    Open = table.Column<decimal>(nullable: false),
                    Resistance = table.Column<decimal>(nullable: true),
                    SD20 = table.Column<decimal>(nullable: true),
                    SignalLine = table.Column<decimal>(nullable: true),
                    Support = table.Column<decimal>(nullable: true),
                    TrendHigh = table.Column<int>(nullable: true),
                    TrendLow = table.Column<int>(nullable: true),
                    Volume = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockPrice", x => new { x.StockID, x.Date });
                    table.ForeignKey(
                        name: "FK_StockPrice_Stock_StockID",
                        column: x => x.StockID,
                        principalTable: "Stock",
                        principalColumn: "StockID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockWatchList",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(nullable: false),
                    StockID = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockWatchList", x => new { x.ApplicationUserId, x.StockID });
                    table.ForeignKey(
                        name: "FK_StockWatchList_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockWatchList_Stock_StockID",
                        column: x => x.StockID,
                        principalTable: "Stock",
                        principalColumn: "StockID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StockWatchList_StockID",
                table: "StockWatchList",
                column: "StockID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "StockPrice");

            migrationBuilder.DropTable(
                name: "StockWatchList");

            migrationBuilder.DropTable(
                name: "Stock");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MembershipType",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");
        }
    }
}
