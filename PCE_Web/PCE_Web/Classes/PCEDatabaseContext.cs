using Microsoft.EntityFrameworkCore;

namespace PCE_Web.Classes
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
        public virtual DbSet<SavedItems> SavedItems { get; set; }
        public virtual DbSet<ShopRatingTable> ShopRatingTable { get; set; }
        public virtual DbSet<UserData> UserData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
               optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=PCEDatabase;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CommentsTable>(entity =>
            {
                entity.HasKey(e => e.CommentId)
                    .HasName("PK__Comments__C3B4DFCA25F7DE51");

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<ItemsTable>(entity =>
            {
                entity.HasKey(e => e.ItemId)
                    .HasName("PK__ItemsTab__727E838B2A314DDC");

                entity.Property(e => e.ImgUrl).HasMaxLength(100);

                entity.Property(e => e.ItemName).HasMaxLength(50);

                entity.Property(e => e.Keyword).HasMaxLength(50);

                entity.Property(e => e.PageUrl).HasMaxLength(100);

                entity.Property(e => e.Price).HasMaxLength(50);

                entity.Property(e => e.ShopName).HasMaxLength(50);
            });

            modelBuilder.Entity<SavedItems>(entity =>
            {
                entity.HasKey(e => e.SavedItemId)
                    .HasName("PK__SavedIte__1CBC88C899DD42A5");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ImgUrl).HasMaxLength(50);

                entity.Property(e => e.ItemName).HasMaxLength(50);

                entity.Property(e => e.PageUrl).HasMaxLength(50);

                entity.Property(e => e.Price).HasMaxLength(50);

                entity.Property(e => e.ShopName).HasMaxLength(50);
            });

            modelBuilder.Entity<ShopRatingTable>(entity =>
            {
                entity.HasKey(e => e.ShopId)
                    .HasName("PK__ShopRati__67C557C99AB57AE8");

                entity.Property(e => e.ShopName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<UserData>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__UserData__1788CC4C3DAC58AC");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PasswordSalt)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
