using Microsoft.EntityFrameworkCore.Migrations;

namespace Emmy.Data.Migrations
{
    public partial class UpdateUserCollectionSpelling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "type",
                table: "user_collections",
                newName: "category");

            migrationBuilder.RenameIndex(
                name: "ix_user_collections_user_id_type_item_id",
                table: "user_collections",
                newName: "ix_user_collections_user_id_category_item_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "category",
                table: "user_collections",
                newName: "type");

            migrationBuilder.RenameIndex(
                name: "ix_user_collections_user_id_category_item_id",
                table: "user_collections",
                newName: "ix_user_collections_user_id_type_item_id");
        }
    }
}
