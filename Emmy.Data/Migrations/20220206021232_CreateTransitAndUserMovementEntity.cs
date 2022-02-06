using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emmy.Data.Migrations
{
    public partial class CreateTransitAndUserMovementEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "transits",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    departure = table.Column<byte>(type: "smallint", nullable: false),
                    destination = table.Column<byte>(type: "smallint", nullable: false),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    price = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transits", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_movements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    departure = table.Column<byte>(type: "smallint", nullable: false),
                    destination = table.Column<byte>(type: "smallint", nullable: false),
                    arrival = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_movements", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_movements_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_transits_departure_destination",
                table: "transits",
                columns: new[] { "departure", "destination" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_movements_user_id",
                table: "user_movements",
                column: "user_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transits");

            migrationBuilder.DropTable(
                name: "user_movements");
        }
    }
}
