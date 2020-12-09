using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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

        public virtual DbSet<Comments> Comments { get; set; }
        public virtual DbSet<Reports> Reports { get; set; }
        public virtual DbSet<SavedExceptions> SavedExceptions { get; set; }
        public virtual DbSet<SavedItems> SavedItems { get; set; }
        public virtual DbSet<UserData> UserData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comments>(entity =>
            {
                entity.HasKey(e => e.CommentId)
                    .HasName("PK__tmp_ms_x__C3B4DFCAA5996E3B");

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Reports>(entity =>
            {
                entity.Property(e => e.Comment).IsRequired();

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<SavedExceptions>(entity =>
            {
                entity.HasKey(e => e.SavedExceptionId)
                    .HasName("PK__tmp_ms_x__1365928E15B63AAB");

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.Source).IsRequired();

                entity.Property(e => e.StackTrace).IsRequired();
            });

            modelBuilder.Entity<SavedItems>(entity =>
            {
                entity.HasKey(e => e.SavedItemId)
                    .HasName("PK__tmp_ms_x__1CBC88C8018F2B66");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.ShopName).HasMaxLength(255);
            });

            modelBuilder.Entity<UserData>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__tmp_ms_x__1788CC4C60E97E98");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.PasswordSalt)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(1);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
