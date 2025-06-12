using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbsale")]
    [Index("Seller", Name = "Seller")]
    public partial class TbSale {
        [Key]
        public int Id { get; set; }

        public int Seller { get; set; }

        [Precision(10, 2)]
        public decimal SubTotal { get; set; }

        [Precision(10, 2)]
        public decimal IVA { get; set; }

        [Precision(10, 2)]
        public decimal Total { get; set; }

        [Precision(10, 2)]
        public decimal Pay { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }

        [ForeignKey("Seller")]
        [InverseProperty("TbSales")]
        public virtual TbUser SellerNavigation { get; set; } = null!;

        [InverseProperty("SaleNavigation")]
        public virtual ICollection<TbDeliveryDetail> TbDeliveryDetails { get; set; } = new List<TbDeliveryDetail>();

        [InverseProperty("SaleNavigation")]
        public virtual ICollection<TbSaleDetail> TbSaleDetails { get; set; } = new List<TbSaleDetail>();
    }
}