using Kikis_back_refaccionaria.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kikis_back_refaccionaria.Infrastructure.Data {

    public partial class KikisDbContext : DbContext {

        public KikisDbContext(DbContextOptions<KikisDbContext> options) : base(options) {}

        public virtual DbSet<TbClient> TbClients { get; set; }

        public virtual DbSet<TbDeliveryDetail> TbDeliveryDetails { get; set; }

        public virtual DbSet<TbDeliveryDetailsStatus> TbDeliveryDetailsStatuses { get; set; }

        public virtual DbSet<TbInvoice> TbInvoices { get; set; }

        public virtual DbSet<TbKit> TbKits { get; set; }

        public virtual DbSet<tbmodule> tbmodules { get; set; }

        public virtual DbSet<TbPermission> TbPermissions { get; set; }

        public virtual DbSet<TbProduct> TbProducts { get; set; }

        public virtual DbSet<TbProductBrand> TbProductBrands { get; set; }

        public virtual DbSet<TbProductCategory> TbProductCategories { get; set; }

        public virtual DbSet<TbProductHallway> TbProductHallways { get; set; }

        public virtual DbSet<TbProductKit> TbProductKits { get; set; }

        public virtual DbSet<TbProductLevel> TbProductLevels { get; set; }

        public virtual DbSet<TbProductShelf> TbProductShelves { get; set; }

        public virtual DbSet<TbProductSupplier> TbProductSuppliers { get; set; }

        public virtual DbSet<TbRol> TbRols { get; set; }

        public virtual DbSet<TbSale> TbSales { get; set; }

        public virtual DbSet<TbSaleDetail> TbSaleDetails { get; set; }

        public virtual DbSet<TbSupplier> TbSuppliers { get; set; }

        public virtual DbSet<TbTrackDelivery> TbTrackDeliveries { get; set; }

        public virtual DbSet<TbTrack> TbTracks { get; set; }

        public virtual DbSet<TbTrackStatus> TbTrackStatuses { get; set; }

        public virtual DbSet<TbUser> TbUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<TbClient>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
            });

            modelBuilder.Entity<TbDeliveryDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.HasOne(d => d.SaleNavigation).WithMany(p => p.TbDeliveryDetails)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbdeliverydetails_ibfk_2");

                entity.HasOne(d => d.StatusNavigation).WithMany(p => p.TbDeliveryDetails)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbdeliverydetails_ibfk_3");
            });

            modelBuilder.Entity<TbDeliveryDetailsStatus>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
            });

            modelBuilder.Entity<TbInvoice>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.HasOne(d => d.SaleNavigation).WithMany(p => p.TbInvoices)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbinvoices_ibfk_1");
            });

            modelBuilder.Entity<TbKit>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
            });

            modelBuilder.Entity<tbmodule>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
            });

            modelBuilder.Entity<TbPermission>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.HasOne(d => d.ModuleNavigation).WithMany(p => p.TbPermissions)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbpermission_ibfk_2");

                entity.HasOne(d => d.RolNavigation).WithMany(p => p.TbPermissions)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbpermission_ibfk_1");
            });

            modelBuilder.Entity<TbProduct>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.Property(e => e.Discount).HasDefaultValueSql("'0.00'");
                entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");

                entity.HasOne(d => d.BrandNavigation).WithMany(p => p.TbProducts)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbproduct_ibfk_1");

                entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.TbProducts)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbproduct_ibfk_2");

                entity.HasOne(d => d.HallwayNavigation).WithMany(p => p.TbProducts)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbproduct_ibfk_3");

                entity.HasOne(d => d.LevelNavigation).WithMany(p => p.TbProducts)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbproduct_ibfk_5");

                entity.HasOne(d => d.ShelfNavigation).WithMany(p => p.TbProducts)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbproduct_ibfk_4");
            });

            modelBuilder.Entity<TbProductBrand>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
            });

            modelBuilder.Entity<TbProductCategory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
            });

            modelBuilder.Entity<TbProductHallway>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
            });

            modelBuilder.Entity<TbProductKit>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.HasOne(d => d.KitNavigation).WithMany(p => p.TbProductKits)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbproductkit_ibfk_2");

                entity.HasOne(d => d.ProductNavigation).WithMany(p => p.TbProductKits)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbproductkit_ibfk_1");
            });

            modelBuilder.Entity<TbProductLevel>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
            });

            modelBuilder.Entity<TbProductShelf>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
            });

            modelBuilder.Entity<TbProductSupplier>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.HasOne(d => d.ProductNavigation).WithMany(p => p.TbProductSuppliers)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbproductsupplier_ibfk_1");

                entity.HasOne(d => d.SupplierNavigation).WithMany(p => p.TbProductSuppliers)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbproductsupplier_ibfk_2");
            });

            modelBuilder.Entity<TbRol>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
            });

            modelBuilder.Entity<TbSale>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.HasOne(d => d.SellerNavigation).WithMany(p => p.TbSales)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbsale_ibfk_1");
            });

            modelBuilder.Entity<TbSaleDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.HasOne(d => d.ProductNavigation).WithMany(p => p.TbSaleDetails)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbsaledetail_ibfk_2");

                entity.HasOne(d => d.SaleNavigation).WithMany(p => p.TbSaleDetails)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbsaledetail_ibfk_1");
            });

            modelBuilder.Entity<TbSupplier>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
            });

            modelBuilder.Entity<TbTrackDelivery>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.HasOne(d => d.DeliveryNavigation).WithMany(p => p.TbTrackDeliveries)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbtrackdelivery_ibfk_2");

                entity.HasOne(d => d.TrackNavigation).WithMany(p => p.TbTrackDeliveries)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbtrackdelivery_ibfk_1");
            });

            modelBuilder.Entity<TbTrack>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.HasOne(d => d.StatusNavigation).WithMany(p => p.TbTracks)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbtrack_ibfk_2");

                entity.HasOne(d => d.UserNavigation).WithMany(p => p.TbTracks)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbtrack_ibfk_1");
            });

            modelBuilder.Entity<TbTrackStatus>(entity => {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
            });

            modelBuilder.Entity<TbUser>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.HasOne(d => d.RolNavigation).WithMany(p => p.TbUsers)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tbuser_ibfk_1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
