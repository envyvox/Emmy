using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emmy.Data.Migrations
{
    public partial class CreateLocalAndModulesEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "localizations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<byte>(type: "smallint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    single = table.Column<string>(type: "text", nullable: false),
                    @double = table.Column<string>(name: "double", type: "text", nullable: false),
                    multiply = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_localizations", x => x.id);
                });

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
                name: "ix_localizations_category_name",
                table: "localizations",
                columns: new[] { "category", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_world_modules_module",
                table: "world_modules",
                column: "module",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "localizations");

            migrationBuilder.DropTable(
                name: "world_modules");
        }
    }
}
