using Microsoft.EntityFrameworkCore.Migrations;

namespace Emmy.Data.Migrations
{
    public partial class UpdateUserAddTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "title",
                table: "users",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "title",
                table: "users");
        }
    }
}
