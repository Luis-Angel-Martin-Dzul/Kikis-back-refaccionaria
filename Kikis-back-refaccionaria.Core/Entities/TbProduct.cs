using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbproduct")]
    [Index("Brand", Name = "Brand")]
    [Index("Category", Name = "Category")]
    [Index("Hallway", Name = "Hallway")]
    [Index("Level", Name = "Level")]
    [Index("Shelf", Name = "Shelf")]
    public partial class TbProduct {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(15)]
        public string Barcode { get; set; } = null!;

        public int Brand { get; set; }

        public int Category { get; set; }

        public int Quantity { get; set; }

        [Precision(10, 2)]
        public decimal Price { get; set; }

        [Precision(5, 2)]
        public decimal? Discount { get; set; }

        [StringLength(255)]
        public string? Path { get; set; }

        public int Hallway { get; set; }

        public int Shelf { get; set; }

        public int Level { get; set; }

        public bool IsActive { get; set; }

        [ForeignKey("Brand")]
        [InverseProperty("TbProducts")]
        public virtual TbProductBrand BrandNavigation { get; set; } = null!;

        [ForeignKey("Category")]
        [InverseProperty("TbProducts")]
        public virtual TbProductCategory CategoryNavigation { get; set; } = null!;

        [ForeignKey("Hallway")]
        [InverseProperty("TbProducts")]
        public virtual TbProductHallway HallwayNavigation { get; set; } = null!;

        [ForeignKey("Level")]
        [InverseProperty("TbProducts")]
        public virtual TbProductLevel LevelNavigation { get; set; } = null!;

        [ForeignKey("Shelf")]
        [InverseProperty("TbProducts")]
        public virtual TbProductShelf ShelfNavigation { get; set; } = null!;

        [InverseProperty("ProductNavigation")]
        public virtual ICollection<TbProductKit> TbProductKits { get; set; } = new List<TbProductKit>();

        [InverseProperty("ProductNavigation")]
        public virtual ICollection<TbProductSupplier> TbProductSuppliers { get; set; } = new List<TbProductSupplier>();

        [InverseProperty("ProductNavigation")]
        public virtual ICollection<TbSaleDetail> TbSaleDetails { get; set; } = new List<TbSaleDetail>();
    }
}