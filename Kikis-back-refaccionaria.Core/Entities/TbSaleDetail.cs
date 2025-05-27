using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbsaledetail")]
    [Index("Product", Name = "Product")]
    [Index("Sale", Name = "Sale")]
    public partial class TbSaleDetail {
        [Key]
        public int Id {
            get; set;
        }

        public int Sale {
            get; set;
        }

        public int Product {
            get; set;
        }

        [Precision(10, 2)]
        public decimal Price {
            get; set;
        }

        [Precision(10, 2)]
        public decimal PriceUnit {
            get; set;
        }

        public int Quantity {
            get; set;
        }

        [Precision(10, 2)]
        public decimal Total {
            get; set;
        }

        [ForeignKey("Product")]
        [InverseProperty("TbSaleDetails")]
        public virtual TbProduct ProductNavigation { get; set; } = null!;

        [ForeignKey("Sale")]
        [InverseProperty("TbSaleDetails")]
        public virtual TbSale SaleNavigation { get; set; } = null!;
    }
}