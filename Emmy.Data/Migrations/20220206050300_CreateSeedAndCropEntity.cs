using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emmy.Data.Migrations
{
    public partial class CreateSeedAndCropEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "seeds",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    season = table.Column<byte>(type: "smallint", nullable: false),
                    growth_days = table.Column<long>(type: "bigint", nullable: false),
                    re_growth_days = table.Column<long>(type: "bigint", nullable: false),
                    is_multiply = table.Column<bool>(type: "boolean", nullable: false),
                    price = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_seeds", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "crops",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<long>(type: "bigint", nullable: false),
                    seed_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_crops", x => x.id);
                    table.ForeignKey(
                        name: "fk_crops_seeds_seed_id",
                        column: x => x.seed_id,
                        principalTable: "seeds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_crops_name",
                table: "crops",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_crops_seed_id",
                table: "crops",
                column: "seed_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_seeds_name",
                table: "seeds",
                column: "name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "crops");

            migrationBuilder.DropTable(
                name: "seeds");
        }
    }
}
