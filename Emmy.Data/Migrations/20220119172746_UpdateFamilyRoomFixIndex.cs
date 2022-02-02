using Microsoft.EntityFrameworkCore.Migrations;

namespace Emmy.Data.Migrations
{
    public partial class UpdateFamilyRoomFixIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_family_rooms_user_id",
                table: "family_rooms");

            migrationBuilder.CreateIndex(
                name: "ix_family_rooms_user_id",
                table: "family_rooms",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_family_rooms_user_id",
                table: "family_rooms");

            migrationBuilder.CreateIndex(
                name: "ix_family_rooms_user_id",
                table: "family_rooms",
                column: "user_id",
                unique: true);
        }
    }
}
