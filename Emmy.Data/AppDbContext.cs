using System;
using Emmy.Data.Converters;
using Emmy.Data.Entities;
using Emmy.Data.Entities.Discord;
using Emmy.Data.Entities.User;
using Emmy.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Emmy.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            modelBuilder.UseEntityTypeConfiguration<AppDbContext>();
            modelBuilder.UseSnakeCaseNamingConvention();
            modelBuilder.UseValueConverterForType<DateTime>(new DateTimeUtcKindConverter());
        }

        public DbSet<ContentMessage> ContentMessages { get; set; }
        public DbSet<ContentVote> ContentVotes { get; set; }
        public DbSet<PrivateRoom> PrivateRooms { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<LoveRoom> LoveRooms { get; set; }
        public DbSet<UserGenderRequest> UserGenderRequests { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserVoice> UserVoices { get; set; }

        public DbSet<Banner> Banners { get; set; }
        public DbSet<Crop> Crops { get; set; }
        public DbSet<Fish> Fishes { get; set; }
        public DbSet<Fraction> Fractions { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<Localization> Localizations { get; set; }
        public DbSet<Relationship> Relationships { get; set; }
        public DbSet<Seed> Seeds { get; set; }
        public DbSet<ShopRole> ShopRoles { get; set; }
        public DbSet<Transit> Transits { get; set; }
        public DbSet<WorldProperty> WorldProperties { get; set; }
        public DbSet<WorldState> WorldStates { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<UserBanner> UserBanners { get; set; }
        public DbSet<UserBuilding> UserBuildings { get; set; }
        public DbSet<UserCollection> UserCollections { get; set; }
        public DbSet<UserContainer> UserContainers { get; set; }
        public DbSet<UserCooldown> UserCooldowns { get; set; }
        public DbSet<UserCrop> UserCrops { get; set; }
        public DbSet<UserCurrency> UserCurrencies { get; set; }
        public DbSet<UserDailyReward> UserDailyRewards { get; set; }
        public DbSet<UserDonation> UserDonations { get; set; }
        public DbSet<UserFarm> UserFarms { get; set; }
        public DbSet<UserFish> UserFishes { get; set; }
        public DbSet<UserHangfireJob> UserHangfireJobs { get; set; }
        public DbSet<UserKey> UserKeys { get; set; }
        public DbSet<UserMovement> UserMovements { get; set; }
        public DbSet<UserPremium> UserPremiums { get; set; }
        public DbSet<UserReferrer> UserReferrers { get; set; }
        public DbSet<UserSeed> UserSeeds { get; set; }
        public DbSet<UserStatistic> UserStatistics { get; set; }
        public DbSet<UserTitle> UserTitles { get; set; }
    }
}