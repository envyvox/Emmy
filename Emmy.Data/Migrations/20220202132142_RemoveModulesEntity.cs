using Microsoft.EntityFrameworkCore.Migrations;

namespace Emmy.Data.Migrations
{
    public partial class RemoveModulesEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "world_modules");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "world_modules",
                columns: table => new
                {
                    module = table.Column<byte>(type: "smallint", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_world_modules", x => x.module);
                });

            migrationBuilder.CreateIndex(
                name: "ix_world_modules_module",
                table: "world_modules",
                column: "module",
                unique: true);
        }
    }
}
