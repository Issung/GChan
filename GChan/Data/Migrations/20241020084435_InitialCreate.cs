using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GChan.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoardData",
                columns: table => new
                {
                    Site = table.Column<int>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    GreatestThreadId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardData", x => new { x.Site, x.Code });
                });

            migrationBuilder.CreateTable(
                name: "ThreadData",
                columns: table => new
                {
                    Site = table.Column<int>(type: "INTEGER", nullable: false),
                    BoardCode = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", nullable: true),
                    FileCount = table.Column<int>(type: "INTEGER", nullable: true),
                    LastScrape = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    SavedAssetIds = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreadData", x => new { x.Site, x.BoardCode, x.Id });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoardData");

            migrationBuilder.DropTable(
                name: "ThreadData");
        }
    }
}
