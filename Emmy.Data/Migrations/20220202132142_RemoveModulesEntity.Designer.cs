﻿// <auto-generated />
using System;
using Emmy.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Emmy.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20220202132142_RemoveModulesEntity")]
    partial class RemoveModulesEntity
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Emmy.Data.Entities.Banner", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<long>("Price")
                        .HasColumnType("bigint")
                        .HasColumnName("price");

                    b.Property<byte>("Rarity")
                        .HasColumnType("smallint")
                        .HasColumnName("rarity");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("url");

                    b.HasKey("Id")
                        .HasName("pk_banners");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_banners_name");

                    b.ToTable("banners");
                });

            modelBuilder.Entity("Emmy.Data.Entities.Discord.ContentMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<long>("ChannelId")
                        .HasColumnType("bigint")
                        .HasColumnName("channel_id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<long>("MessageId")
                        .HasColumnType("bigint")
                        .HasColumnName("message_id");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_content_messages");

                    b.HasIndex("UserId", "ChannelId", "MessageId")
                        .HasDatabaseName("ix_content_messages_user_id_channel_id_message_id");

                    b.ToTable("content_messages");
                });

            modelBuilder.Entity("Emmy.Data.Entities.Discord.ContentVote", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("ContentMessageId")
                        .HasColumnType("uuid")
                        .HasColumnName("content_message_id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.Property<byte>("Vote")
                        .HasColumnType("smallint")
                        .HasColumnName("vote");

                    b.HasKey("Id")
                        .HasName("pk_content_votes");

                    b.HasIndex("ContentMessageId")
                        .HasDatabaseName("ix_content_votes_content_message_id");

                    b.HasIndex("UserId", "ContentMessageId", "Vote")
                        .HasDatabaseName("ix_content_votes_user_id_content_message_id_vote");

                    b.ToTable("content_votes");
                });

            modelBuilder.Entity("Emmy.Data.Entities.Discord.Image", b =>
                {
                    b.Property<byte>("Type")
                        .HasColumnType("smallint")
                        .HasColumnName("type");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("url");

                    b.HasKey("Type")
                        .HasName("pk_images");

                    b.HasIndex("Type")
                        .HasDatabaseName("ix_images_type");

                    b.ToTable("images");
                });

            modelBuilder.Entity("Emmy.Data.Entities.Discord.LoveRoom", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<long>("ChannelId")
                        .HasColumnType("bigint")
                        .HasColumnName("channel_id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset>("Expiration")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expiration");

                    b.Property<Guid>("RelationshipId")
                        .HasColumnType("uuid")
                        .HasColumnName("relationship_id");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_love_rooms");

                    b.HasIndex("RelationshipId")
                        .IsUnique()
                        .HasDatabaseName("ix_love_rooms_relationship_id");

                    b.ToTable("love_rooms");
                });

            modelBuilder.Entity("Emmy.Data.Entities.Discord.PrivateRoom", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<long>("ChannelId")
                        .HasColumnType("bigint")
                        .HasColumnName("channel_id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset>("Expiration")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expiration");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_private_rooms");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_private_rooms_user_id");

                    b.ToTable("private_rooms");
                });

            modelBuilder.Entity("Emmy.Data.Entities.Discord.UserGenderRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<long?>("ModeratorId")
                        .HasColumnType("bigint")
                        .HasColumnName("moderator_id");

                    b.Property<byte>("State")
                        .HasColumnType("smallint")
                        .HasColumnName("state");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_gender_requests");

                    b.HasIndex("ModeratorId")
                        .HasDatabaseName("ix_user_gender_requests_moderator_id");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasDatabaseName("ix_user_gender_requests_user_id");

                    b.ToTable("user_gender_requests");
                });

            modelBuilder.Entity("Emmy.Data.Entities.Discord.UserRole", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset?>("Expiration")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expiration");

                    b.Property<bool>("IsPersonal")
                        .HasColumnType("boolean")
                        .HasColumnName("is_personal");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint")
                        .HasColumnName("role_id");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_roles");

                    b.HasIndex("UserId", "RoleId")
                        .IsUnique()
                        .HasDatabaseName("ix_user_roles_user_id_role_id");

                    b.ToTable("user_roles");
                });

            modelBuilder.Entity("Emmy.Data.Entities.Discord.UserVoice", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<long>("ChannelId")
                        .HasColumnType("bigint")
                        .HasColumnName("channel_id");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_voices");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasDatabaseName("ix_user_voices_user_id");

                    b.ToTable("user_voices");
                });

            modelBuilder.Entity("Emmy.Data.Entities.DynamicShopBanner", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("BannerId")
                        .HasColumnType("uuid")
                        .HasColumnName("banner_id");

                    b.HasKey("Id")
                        .HasName("pk_dynamic_shop_banners");

                    b.HasIndex("BannerId")
                        .IsUnique()
                        .HasDatabaseName("ix_dynamic_shop_banners_banner_id");

                    b.ToTable("dynamic_shop_banners");
                });

            modelBuilder.Entity("Emmy.Data.Entities.Key", b =>
                {
                    b.Property<byte>("Type")
                        .HasColumnType("smallint")
                        .HasColumnName("type");

                    b.Property<long>("Price")
                        .HasColumnType("bigint")
                        .HasColumnName("price");

                    b.HasKey("Type")
                        .HasName("pk_keys");

                    b.HasIndex("Type")
                        .IsUnique()
                        .HasDatabaseName("ix_keys_type");

                    b.ToTable("keys");
                });

            modelBuilder.Entity("Emmy.Data.Entities.Localization", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<byte>("Category")
                        .HasColumnType("smallint")
                        .HasColumnName("category");

                    b.Property<string>("Double")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("double");

                    b.Property<string>("Multiply")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("multiply");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Single")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("single");

                    b.HasKey("Id")
                        .HasName("pk_localizations");

                    b.HasIndex("Category", "Name")
                        .IsUnique()
                        .HasDatabaseName("ix_localizations_category_name");

                    b.ToTable("localizations");
                });

            modelBuilder.Entity("Emmy.Data.Entities.Relationship", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<long>("User1Id")
                        .HasColumnType("bigint")
                        .HasColumnName("user1_id");

                    b.Property<long>("User2Id")
                        .HasColumnType("bigint")
                        .HasColumnName("user2_id");

                    b.HasKey("Id")
                        .HasName("pk_relationships");

                    b.HasIndex("User1Id")
                        .IsUnique()
                        .HasDatabaseName("ix_relationships_user1_id");

                    b.HasIndex("User2Id")
                        .IsUnique()
                        .HasDatabaseName("ix_relationships_user2_id");

                    b.ToTable("relationships");
                });

            modelBuilder.Entity("Emmy.Data.Entities.ShopRole", b =>
                {
                    b.Property<long>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("role_id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<long>("Price")
                        .HasColumnType("bigint")
                        .HasColumnName("price");

                    b.HasKey("RoleId")
                        .HasName("pk_shop_roles");

                    b.HasIndex("RoleId")
                        .IsUnique()
                        .HasDatabaseName("ix_shop_roles_role_id");

                    b.ToTable("shop_roles");
                });

            modelBuilder.Entity("Emmy.Data.Entities.User.User", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("About")
                        .HasColumnType("text")
                        .HasColumnName("about");

                    b.Property<string>("CommandColor")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("command_color");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<byte>("Cube")
                        .HasColumnType("smallint")
                        .HasColumnName("cube");

                    b.Property<byte>("Fraction")
                        .HasColumnType("smallint")
                        .HasColumnName("fraction");

                    b.Property<byte>("Gender")
                        .HasColumnType("smallint")
                        .HasColumnName("gender");

                    b.Property<bool>("IsPremium")
                        .HasColumnType("boolean")
                        .HasColumnName("is_premium");

                    b.Property<long>("Level")
                        .HasColumnType("bigint")
                        .HasColumnName("level");

                    b.Property<byte>("Location")
                        .HasColumnType("smallint")
                        .HasColumnName("location");

                    b.Property<bool>("OnGuild")
                        .HasColumnType("boolean")
                        .HasColumnName("on_guild");

                    b.Property<byte>("Title")
                        .HasColumnType("smallint")
                        .HasColumnName("title");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<long>("Xp")
                        .HasColumnType("bigint")
                        .HasColumnName("xp");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_users_id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("Emmy.Data.Entities.User.UserBanner", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("BannerId")
                        .HasColumnType("uuid")
                        .HasColumnName("banner_id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_banners");

                    b.HasIndex("BannerId")
                        .HasDatabaseName("ix_user_banners_banner_id");

                    b.HasIndex("UserId", "BannerId")
                        .IsUnique()
                        .HasDatabaseName("ix_user_banners_user_id_banner_id");

                    b.ToTable("user_banners");
                });

            modelBuilder.Entity("Emmy.Data.Entities.User.UserCurrency", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<long>("Amount")
                        .HasColumnType("bigint")
                        .HasColumnName("amount");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_currencies");

                    b.HasIndex("UserId", "Type")
                        .IsUnique()
                        .HasDatabaseName("ix_user_currencies_user_id_type");

                    b.ToTable("user_currencies");
                });

            modelBuilder.Entity("Emmy.Data.Entities.User.UserDonation", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<long>("Amount")
                        .HasColumnType("bigint")
                        .HasColumnName("amount");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_donations");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasDatabaseName("ix_user_donations_user_id");

                    b.ToTable("user_donations");
                });

            modelBuilder.Entity("Emmy.Data.Entities.User.UserKey", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<long>("Amount")
                        .HasColumnType("bigint")
                        .HasColumnName("amount");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<byte>("Type")
                        .HasColumnType("smallint")
                        .HasColumnName("type");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_keys");

                    b.HasIndex("Type")
                        .HasDatabaseName("ix_user_keys_type");

                    b.HasIndex("UserId", "Type")
                        .IsUnique()
                        .HasDatabaseName("ix_user_keys_user_id_type");

                    b.ToTable("user_keys");
                });

            modelBuilder.Entity("Emmy.Data.Entities.User.UserPremium", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset>("Expiration")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expiration");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_premiums");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasDatabaseName("ix_user_premiums_user_id");

                    b.ToTable("user_premiums");
                });

            modelBuilder.Entity("Emmy.Data.Entities.User.UserStatistic", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<long>("Amount")
                        .HasColumnType("bigint")
                        .HasColumnName("amount");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<byte>("Type")
                        .HasColumnType("smallint")
                        .HasColumnName("type");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_statistics");

                    b.HasIndex("UserId", "Type")
                        .IsUnique()
                        .HasDatabaseName("ix_user_statistics_user_id_type");

                    b.ToTable("user_statistics");
                });

            modelBuilder.Entity("Emmy.Data.Entities.WorldProperty", b =>
                {
                    b.Property<byte>("Type")
                        .HasColumnType("smallint")
                        .HasColumnName("type");

                    b.Property<long>("Value")
                        .HasColumnType("bigint")
                        .HasColumnName("value");

                    b.HasKey("Type")
                        .HasName("pk_world_properties");

                    b.HasIndex("Type")
                        .IsUnique()
                        .HasDatabaseName("ix_world_properties_type");

                    b.ToTable("world_properties");
                });

            modelBuilder.Entity("Emmy.Data.Entities.WorldState", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<byte>("CurrentSeason")
                        .HasColumnType("smallint")
                        .HasColumnName("current_season");

                    b.Property<byte>("WeatherToday")
                        .HasColumnType("smallint")
                        .HasColumnName("weather_today");

                    b.Property<byte>("WeatherTomorrow")
                        .HasColumnType("smallint")
                        .HasColumnName("weather_tomorrow");

                    b.HasKey("Id")
                        .HasName("pk_world_states");

                    b.ToTable("world_states");
                });

            modelBuilder.Entity("Emmy.Data.Entities.Discord.ContentMessage", b =>
                {
                    b.HasOne("Emmy.Data.Entities.User.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_content_messages_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Emmy.Data.Entities.Discord.ContentVote", b =>
                {
                    b.HasOne("Emmy.Data.Entities.Discord.ContentMessage", "ContentMessage")
                        .WithMany()
                        .HasForeignKey("ContentMessageId")
                        .HasConstraintName("fk_content_votes_content_messages_content_message_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Emmy.Data.Entities.User.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_content_votes_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ContentMessage");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Emmy.Data.Entities.Discord.LoveRoom", b =>
                {
                    b.HasOne("Emmy.Data.Entities.Relationship", "Relationship")
                        .WithOne()
                        .HasForeignKey("Emmy.Data.Entities.Discord.LoveRoom", "RelationshipId")
                        .HasConstraintName("fk_love_rooms_relationships_relationship_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Relationship");
                });

            modelBuilder.Entity("Emmy.Data.Entities.Discord.PrivateRoom", b =>
                {
                    b.HasOne("Emmy.Data.Entities.User.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_private_rooms_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Emmy.Data.Entities.Discord.UserGenderRequest", b =>
                {
                    b.HasOne("Emmy.Data.Entities.User.User", "Moderator")
                        .WithMany()
                        .HasForeignKey("ModeratorId")
                        .HasConstraintName("fk_user_gender_requests_users_moderator_id");

                    b.HasOne("Emmy.Data.Entities.User.User", "User")
                        .WithOne()
                        .HasForeignKey("Emmy.Data.Entities.Discord.UserGenderRequest", "UserId")
                        .HasConstraintName("fk_user_gender_requests_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Moderator");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Emmy.Data.Entities.Discord.UserRole", b =>
                {
                    b.HasOne("Emmy.Data.Entities.User.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_roles_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Emmy.Data.Entities.Discord.UserVoice", b =>
                {
                    b.HasOne("Emmy.Data.Entities.User.User", "User")
                        .WithOne()
                        .HasForeignKey("Emmy.Data.Entities.Discord.UserVoice", "UserId")
                        .HasConstraintName("fk_user_voices_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Emmy.Data.Entities.DynamicShopBanner", b =>
                {
                    b.HasOne("Emmy.Data.Entities.Banner", "Banner")
                        .WithOne()
                        .HasForeignKey("Emmy.Data.Entities.DynamicShopBanner", "BannerId")
                        .HasConstraintName("fk_dynamic_shop_banners_banners_banner_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Banner");
                });

            modelBuilder.Entity("Emmy.Data.Entities.Relationship", b =>
                {
                    b.HasOne("Emmy.Data.Entities.User.User", "User1")
                        .WithOne()
                        .HasForeignKey("Emmy.Data.Entities.Relationship", "User1Id")
                        .HasConstraintName("fk_relationships_users_user1_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Emmy.Data.Entities.User.User", "User2")
                        .WithOne()
                        .HasForeignKey("Emmy.Data.Entities.Relationship", "User2Id")
                        .HasConstraintName("fk_relationships_users_user2_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User1");

                    b.Navigation("User2");
                });

            modelBuilder.Entity("Emmy.Data.Entities.User.UserBanner", b =>
                {
                    b.HasOne("Emmy.Data.Entities.Banner", "Banner")
                        .WithMany()
                        .HasForeignKey("BannerId")
                        .HasConstraintName("fk_user_banners_banners_banner_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Emmy.Data.Entities.User.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_banners_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Banner");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Emmy.Data.Entities.User.UserCurrency", b =>
                {
                    b.HasOne("Emmy.Data.Entities.User.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_currencies_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Emmy.Data.Entities.User.UserDonation", b =>
                {
                    b.HasOne("Emmy.Data.Entities.User.User", "User")
                        .WithOne()
                        .HasForeignKey("Emmy.Data.Entities.User.UserDonation", "UserId")
                        .HasConstraintName("fk_user_donations_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Emmy.Data.Entities.User.UserKey", b =>
                {
                    b.HasOne("Emmy.Data.Entities.Key", "Key")
                        .WithMany()
                        .HasForeignKey("Type")
                        .HasConstraintName("fk_user_keys_keys_type")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Emmy.Data.Entities.User.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_keys_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Key");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Emmy.Data.Entities.User.UserPremium", b =>
                {
                    b.HasOne("Emmy.Data.Entities.User.User", "User")
                        .WithOne()
                        .HasForeignKey("Emmy.Data.Entities.User.UserPremium", "UserId")
                        .HasConstraintName("fk_user_premiums_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Emmy.Data.Entities.User.UserStatistic", b =>
                {
                    b.HasOne("Emmy.Data.Entities.User.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_statistics_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
