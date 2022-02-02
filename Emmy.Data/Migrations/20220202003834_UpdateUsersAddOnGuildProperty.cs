using Microsoft.EntityFrameworkCore.Migrations;

namespace Emmy.Data.Migrations
{
    public partial class UpdateUsersAddOnGuildProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "on_guild",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "on_guild",
                table: "users");
        }
    }
}
