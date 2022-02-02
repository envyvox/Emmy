using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emmy.Data.Migrations
{
    public partial class CreateRelationshipsEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_love_rooms_users_user1_id",
                table: "love_rooms");

            migrationBuilder.DropForeignKey(
                name: "fk_love_rooms_users_user2_id",
                table: "love_rooms");

            migrationBuilder.DropIndex(
                name: "ix_love_rooms_user1_id",
                table: "love_rooms");

            migrationBuilder.DropIndex(
                name: "ix_love_rooms_user2_id",
                table: "love_rooms");

            migrationBuilder.DropColumn(
                name: "user1_id",
                table: "love_rooms");

            migrationBuilder.DropColumn(
                name: "user2_id",
                table: "love_rooms");

            migrationBuilder.AlterColumn<int>(
                name: "type",
                table: "user_currencies",
                type: "integer",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AddColumn<Guid>(
                name: "relationship_id",
                table: "love_rooms",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "relationships",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user1_id = table.Column<long>(type: "bigint", nullable: false),
                    user2_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_relationships", x => x.id);
                    table.ForeignKey(
                        name: "fk_relationships_users_user1_id",
                        column: x => x.user1_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_relationships_users_user2_id",
                        column: x => x.user2_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_love_rooms_relationship_id",
                table: "love_rooms",
                column: "relationship_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_relationships_user1_id",
                table: "relationships",
                column: "user1_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_relationships_user2_id",
                table: "relationships",
                column: "user2_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_love_rooms_relationships_relationship_id",
                table: "love_rooms",
                column: "relationship_id",
                principalTable: "relationships",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_love_rooms_relationships_relationship_id",
                table: "love_rooms");

            migrationBuilder.DropTable(
                name: "relationships");

            migrationBuilder.DropIndex(
                name: "ix_love_rooms_relationship_id",
                table: "love_rooms");

            migrationBuilder.DropColumn(
                name: "relationship_id",
                table: "love_rooms");

            migrationBuilder.AlterColumn<byte>(
                name: "type",
                table: "user_currencies",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<long>(
                name: "user1_id",
                table: "love_rooms",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "user2_id",
                table: "love_rooms",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "ix_love_rooms_user1_id",
                table: "love_rooms",
                column: "user1_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_love_rooms_user2_id",
                table: "love_rooms",
                column: "user2_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_love_rooms_users_user1_id",
                table: "love_rooms",
                column: "user1_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_love_rooms_users_user2_id",
                table: "love_rooms",
                column: "user2_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
