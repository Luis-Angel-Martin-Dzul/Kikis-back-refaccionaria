using Kikis_back_refaccionaria.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kikis_back_refaccionaria.Infrastructure.Data;

public partial class KikisDbContext : DbContext
{
    public KikisDbContext() {}

    public KikisDbContext(DbContextOptions<KikisDbContext> options) : base(options) {}

    public virtual DbSet<TbKit> TbKits { get; set; }

    public virtual DbSet<TbModule> TbModules { get; set; }

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

    public virtual DbSet<TbUser> TbUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TbKit>(entity =>
        {
            entity.ToTable("TbKit");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(75)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TbModule>(entity =>
        {
            entity.ToTable("TbModule");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(75)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TbPermission>(entity =>
        {
            entity.ToTable("TbPermission");

            entity.HasOne(d => d.ModuleNavigation).WithMany(p => p.TbPermissions)
                .HasForeignKey(d => d.Module)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbPermission_TbModule");

            entity.HasOne(d => d.RolNavigation).WithMany(p => p.TbPermissions)
                .HasForeignKey(d => d.Rol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbPermission_TbRol");
        });

        modelBuilder.Entity<TbProduct>(entity =>
        {
            entity.ToTable("TbProduct");

            entity.Property(e => e.Barcode)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Discount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Path)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.BrandNavigation).WithMany(p => p.TbProducts)
                .HasForeignKey(d => d.Brand)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbProduct_TbToolBrand");

            entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.TbProducts)
                .HasForeignKey(d => d.Category)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbProduct_TbToolCategory");

            entity.HasOne(d => d.HallwayNavigation).WithMany(p => p.TbProducts)
                .HasForeignKey(d => d.Hallway)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbProduct_TbProductHallway");

            entity.HasOne(d => d.LevelNavigation).WithMany(p => p.TbProducts)
                .HasForeignKey(d => d.Level)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbProduct_TbProductLevel");

            entity.HasOne(d => d.ShelfNavigation).WithMany(p => p.TbProducts)
                .HasForeignKey(d => d.Shelf)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbProduct_TbProductShelf");
        });

        modelBuilder.Entity<TbProductBrand>(entity =>
        {
            entity.ToTable("TbProductBrand");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(75)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TbProductCategory>(entity =>
        {
            entity.ToTable("TbProductCategory");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(75)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TbProductHallway>(entity =>
        {
            entity.ToTable("TbProductHallway");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(75)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TbProductKit>(entity =>
        {
            entity.ToTable("TbProductKit");

            entity.HasOne(d => d.KitNavigation).WithMany(p => p.TbProductKits)
                .HasForeignKey(d => d.Kit)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbProductKit_TbKitr");

            entity.HasOne(d => d.ProductNavigation).WithMany(p => p.TbProductKits)
                .HasForeignKey(d => d.Product)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbProductKit_TbProduct");
        });

        modelBuilder.Entity<TbProductLevel>(entity =>
        {
            entity.ToTable("TbProductLevel");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(75)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TbProductShelf>(entity =>
        {
            entity.ToTable("TbProductShelf");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(75)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TbProductSupplier>(entity =>
        {
            entity.ToTable("TbProductSupplier");

            entity.HasOne(d => d.ProductNavigation).WithMany(p => p.TbProductSuppliers)
                .HasForeignKey(d => d.Product)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbProductSupplier_TbProduct");

            entity.HasOne(d => d.SupplierNavigation).WithMany(p => p.TbProductSuppliers)
                .HasForeignKey(d => d.Supplier)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbProductSupplier_TbSupplier");
        });

        modelBuilder.Entity<TbRol>(entity =>
        {
            entity.ToTable("TbRol");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(75)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TbSale>(entity =>
        {
            entity.ToTable("TbSale");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Iva)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("IVA");
            entity.Property(e => e.Pay).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.SellerNavigation).WithMany(p => p.TbSales)
                .HasForeignKey(d => d.Seller)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbSale_TbUser");
        });

        modelBuilder.Entity<TbSaleDetail>(entity =>
        {
            entity.ToTable("TbSaleDetail");

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PriceUnit).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.SaleNavigation).WithMany(p => p.TbSaleDetails)
                .HasForeignKey(d => d.Sale)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbSaleDetail_TbSale");
        });

        modelBuilder.Entity<TbSupplier>(entity =>
        {
            entity.ToTable("TbSupplier");

            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.BusinessName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Cellphone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Cellphone2)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Curp)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("CURP");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Owner)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Representative)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Rfc)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("RFC");
            entity.Property(e => e.TradeName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TbUser>(entity =>
        {
            entity.ToTable("TbUser");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Curp)
                .HasMaxLength(18)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.RolNavigation).WithMany(p => p.TbUsers)
                .HasForeignKey(d => d.Rol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbUser_TbRol");
        });
    }

}

    