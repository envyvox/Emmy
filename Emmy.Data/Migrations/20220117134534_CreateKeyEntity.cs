using Microsoft.EntityFrameworkCore.Migrations;

namespace Emmy.Data.Migrations
{
    public partial class CreateKeyEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "amount",
                table: "user_keys",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "keys",
                columns: table => new
                {
                    type = table.Column<byte>(type: "smallint", nullable: false),
                    price = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_keys", x => x.type);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_keys_type",
                table: "user_keys",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "ix_keys_type",
                table: "keys",
                column: "type",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_user_keys_keys_type",
                table: "user_keys",
                column: "type",
                principalTable: "keys",
                principalColumn: "type",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_keys_keys_type",
                table: "user_keys");

            migrationBuilder.DropTable(
                name: "keys");

            migrationBuilder.DropIndex(
                name: "ix_user_keys_type",
                table: "user_keys");

            migrationBuilder.DropColumn(
                name: "amount",
                table: "user_keys");
        }
    }
}
