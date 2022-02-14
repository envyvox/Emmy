using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emmy.Data.Migrations
{
    public partial class CreateFractionAndFractionQuestEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fractions",
                columns: table => new
                {
                    type = table.Column<byte>(type: "smallint", nullable: false),
                    points = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fractions", x => x.type);
                });

            migrationBuilder.CreateTable(
                name: "fraction_quests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    fraction_type = table.Column<byte>(type: "smallint", nullable: false),
                    quest = table.Column<byte>(type: "smallint", nullable: false),
                    progress = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fraction_quests", x => x.id);
                    table.ForeignKey(
                        name: "fk_fraction_quests_fractions_fraction_type",
                        column: x => x.fraction_type,
                        principalTable: "fractions",
                        principalColumn: "type",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_fraction_quests_fraction_type",
                table: "fraction_quests",
                column: "fraction_type");

            migrationBuilder.CreateIndex(
                name: "ix_fractions_type",
                table: "fractions",
                column: "type",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fraction_quests");

            migrationBuilder.DropTable(
                name: "fractions");
        }
    }
}
