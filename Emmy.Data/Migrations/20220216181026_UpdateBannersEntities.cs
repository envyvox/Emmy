using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emmy.Data.Migrations
{
    public partial class UpdateBannersEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dynamic_shop_banners");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "banners",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "banners");

            migrationBuilder.CreateTable(
                name: "dynamic_shop_banners",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    banner_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dynamic_shop_banners", x => x.id);
                    table.ForeignKey(
                        name: "fk_dynamic_shop_banners_banners_banner_id",
                        column: x => x.banner_id,
                        principalTable: "banners",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_dynamic_shop_banners_banner_id",
                table: "dynamic_shop_banners",
                column: "banner_id",
                unique: true);
        }
    }
}
