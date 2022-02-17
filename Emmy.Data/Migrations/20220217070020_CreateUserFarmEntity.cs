using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emmy.Data.Migrations
{
    public partial class CreateUserFarmEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_farms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    number = table.Column<long>(type: "bigint", nullable: false),
                    state = table.Column<byte>(type: "smallint", nullable: false),
                    seed_id = table.Column<Guid>(type: "uuid", nullable: true),
                    progress = table.Column<long>(type: "bigint", nullable: false),
                    in_re_growth = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_farms", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_farms_seeds_seed_id",
                        column: x => x.seed_id,
                        principalTable: "seeds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_farms_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_farms_seed_id",
                table: "user_farms",
                column: "seed_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_farms_user_id_number",
                table: "user_farms",
                columns: new[] { "user_id", "number" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_farms");
        }
    }
}
