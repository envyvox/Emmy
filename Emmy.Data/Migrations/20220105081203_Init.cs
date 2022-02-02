using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emmy.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "banners",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    rarity = table.Column<byte>(type: "smallint", nullable: false),
                    price = table.Column<long>(type: "bigint", nullable: false),
                    url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_banners", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    about = table.Column<string>(type: "text", nullable: true),
                    level = table.Column<long>(type: "bigint", nullable: false),
                    xp = table.Column<long>(type: "bigint", nullable: false),
                    fraction = table.Column<byte>(type: "smallint", nullable: false),
                    location = table.Column<byte>(type: "smallint", nullable: false),
                    gender = table.Column<byte>(type: "smallint", nullable: false),
                    cube = table.Column<byte>(type: "smallint", nullable: false),
                    command_color = table.Column<string>(type: "text", nullable: false),
                    is_premium = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "world_properties",
                columns: table => new
                {
                    type = table.Column<byte>(type: "smallint", nullable: false),
                    value = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_world_properties", x => x.type);
                });

            migrationBuilder.CreateTable(
                name: "world_states",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    current_season = table.Column<byte>(type: "smallint", nullable: false),
                    weather_today = table.Column<byte>(type: "smallint", nullable: false),
                    weather_tomorrow = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_world_states", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "content_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    channel_id = table.Column<long>(type: "bigint", nullable: false),
                    message_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_content_messages_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_banners",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    banner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_banners", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_banners_banners_banner_id",
                        column: x => x.banner_id,
                        principalTable: "banners",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_banners_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_currencies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<byte>(type: "smallint", nullable: false),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_currencies", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_currencies_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    role_id = table.Column<long>(type: "bigint", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    expiration = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_roles", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_statistics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<byte>(type: "smallint", nullable: false),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_statistics", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_statistics_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_voices",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    channel_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_voices", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_voices_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "content_votes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    content_message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vote = table.Column<byte>(type: "smallint", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content_votes", x => x.id);
                    table.ForeignKey(
                        name: "fk_content_votes_content_messages_content_message_id",
                        column: x => x.content_message_id,
                        principalTable: "content_messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_content_votes_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_banners_name",
                table: "banners",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_content_messages_user_id_channel_id_message_id",
                table: "content_messages",
                columns: new[] { "user_id", "channel_id", "message_id" });

            migrationBuilder.CreateIndex(
                name: "ix_content_votes_content_message_id",
                table: "content_votes",
                column: "content_message_id");

            migrationBuilder.CreateIndex(
                name: "ix_content_votes_user_id_content_message_id_vote",
                table: "content_votes",
                columns: new[] { "user_id", "content_message_id", "vote" });

            migrationBuilder.CreateIndex(
                name: "ix_user_banners_banner_id",
                table: "user_banners",
                column: "banner_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_banners_user_id_banner_id",
                table: "user_banners",
                columns: new[] { "user_id", "banner_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_currencies_user_id_type",
                table: "user_currencies",
                columns: new[] { "user_id", "type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_user_id_role_id",
                table: "user_roles",
                columns: new[] { "user_id", "role_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_statistics_user_id_type",
                table: "user_statistics",
                columns: new[] { "user_id", "type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_voices_user_id",
                table: "user_voices",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_user_id",
                table: "users",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_world_properties_type",
                table: "world_properties",
                column: "type",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "content_votes");

            migrationBuilder.DropTable(
                name: "user_banners");

            migrationBuilder.DropTable(
                name: "user_currencies");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "user_statistics");

            migrationBuilder.DropTable(
                name: "user_voices");

            migrationBuilder.DropTable(
                name: "world_properties");

            migrationBuilder.DropTable(
                name: "world_states");

            migrationBuilder.DropTable(
                name: "content_messages");

            migrationBuilder.DropTable(
                name: "banners");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
