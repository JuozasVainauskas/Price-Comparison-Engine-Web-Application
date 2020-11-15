using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace PCE_Web.Tables
{
    public partial class PCEDatabaseContext : DbContext
    {
        public PCEDatabaseContext()
        {
        }

        public PCEDatabaseContext(DbContextOptions<PCEDatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CommentsTable> CommentsTable { get; set; }
        public virtual DbSet<ItemsTable> ItemsTable { get; set; }
        public virtual DbSet<ReportsTable> ReportsTable { get; set; }
        public virtual DbSet<SavedExceptions> SavedExceptions { get; set; }
        public virtual DbSet<SavedItems> SavedItems { get; set; }
        public virtual DbSet<ShopRatingTable> ShopRatingTable { get; set; }
        public virtual DbSet<UserData> UserData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();

                optionsBuilder.UseSqlServer(configuration.GetConnectionString("PCEConnectionString"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CommentsTable>(entity =>
            {
                entity.HasKey(e => e.CommentId)
                    .HasName("PK__tmp_ms_x__C3B4DFCA01486395");

                entity.Property(e => e.Date).IsRequired();

                entity.Property(e => e.Email).IsRequired();
            });

            modelBuilder.Entity<ItemsTable>(entity =>
            {
                entity.HasKey(e => e.ItemId)
                    .HasName("PK__ItemsTab__727E838B9BA0A08C");
            });

            modelBuilder.Entity<ReportsTable>(entity =>
            {
                entity.HasKey(e => e.ReportsId)
                    .HasName("PK__tmp_ms_x__37856ECB6CD7432D");

                entity.Property(e => e.Comment).IsRequired();

                entity.Property(e => e.Email).IsRequired();
            });

            modelBuilder.Entity<SavedExceptions>(entity =>
            {
                entity.HasKey(e => e.SavedExceptionId)
                    .HasName("PK__SavedExc__1365928E961A2AFE");

                entity.Property(e => e.Type).IsRequired();
            });

            modelBuilder.Entity<SavedItems>(entity =>
            {
                entity.HasKey(e => e.SavedItemId)
                    .HasName("PK__SavedIte__1CBC88C858E6ACD1");

                entity.Property(e => e.Email).IsRequired();
            });

            modelBuilder.Entity<ShopRatingTable>(entity =>
            {
                entity.HasKey(e => e.ShopId)
                    .HasName("PK__ShopRati__67C557C9184615A1");

                entity.Property(e => e.ShopName).IsRequired();
            });

            modelBuilder.Entity<UserData>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__UserData__1788CC4C5812A428");

                entity.Property(e => e.Email).IsRequired();

                entity.Property(e => e.PasswordHash).IsRequired();

                entity.Property(e => e.PasswordSalt).IsRequired();

                entity.Property(e => e.Role).IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
