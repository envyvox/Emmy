using Microsoft.EntityFrameworkCore.Migrations;

namespace Emmy.Data.Migrations
{
    public partial class UpdateFamilyRoomFixNaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_family_rooms_users_owner_id",
                table: "family_rooms");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "family_rooms",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "ix_family_rooms_owner_id",
                table: "family_rooms",
                newName: "ix_family_rooms_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_family_rooms_users_user_id",
                table: "family_rooms",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_family_rooms_users_user_id",
                table: "family_rooms");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "family_rooms",
                newName: "owner_id");

            migrationBuilder.RenameIndex(
                name: "ix_family_rooms_user_id",
                table: "family_rooms",
                newName: "ix_family_rooms_owner_id");

            migrationBuilder.AddForeignKey(
                name: "fk_family_rooms_users_owner_id",
                table: "family_rooms",
                column: "owner_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
