using System;
using System.Collections.Generic;
using Emmy.Data.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emmy.Data.Migrations
{
    public partial class CreateFishEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fishes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    rarity = table.Column<byte>(type: "smallint", nullable: false),
                    catch_weather = table.Column<byte>(type: "smallint", nullable: false),
                    catch_times_day = table.Column<byte>(type: "smallint", nullable: false),
                    catch_seasons = table.Column<List<Season>>(type: "smallint[]", nullable: false),
                    price = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fishes", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_fishes_name",
                table: "fishes",
                column: "name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fishes");
        }
    }
}
