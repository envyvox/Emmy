using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emmy.Data.Migrations
{
    public partial class UpdateFishesSeasons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "catch_seasons",
                table: "fishes",
                type: "text",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "smallint[]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "catch_seasons",
                table: "fishes",
                type: "smallint[]",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
