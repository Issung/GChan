using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GChan.Data.Migrations
{
    /// <inheritdoc />
    public partial class BoardsLastScrape : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastScrape",
                table: "BoardData",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastScrape",
                table: "BoardData");
        }
    }
}
