using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emmy.Data.Migrations
{
    public partial class CreateUserGenderRequestEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_gender_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    state = table.Column<byte>(type: "smallint", nullable: false),
                    moderator_id = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_gender_requests", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_gender_requests_users_moderator_id",
                        column: x => x.moderator_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_gender_requests_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_gender_requests_moderator_id",
                table: "user_gender_requests",
                column: "moderator_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_gender_requests_user_id",
                table: "user_gender_requests",
                column: "user_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_gender_requests");
        }
    }
}
