using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Index("Delivery", Name = "tbdeliverydetails_ibfk_1_idx")]
    [Index("Sale", Name = "tbdeliverydetails_ibfk_2_idx")]
    [Index("Status", Name = "tbdeliverydetails_ibfk_3_idx")]
    public partial class TbDeliveryDetail {

        [Key]
        public int Id { get; set; }

        public int? Delivery { get; set; }

        public int Sale { get; set; }

        [StringLength(45)]
        public string Responsible { get; set; } = null!;

        [StringLength(200)]
        public string Address { get; set; } = null!;

        [Precision(9, 6)]
        public decimal Latitude { get; set; }

        [Precision(9, 6)]
        public decimal Longitude { get; set; }

        public int Status { get; set; }

        [StringLength(255)]
        public string? Comments { get; set; } = null!;

        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }

        [ForeignKey("Delivery")]
        [InverseProperty("TbDeliveryDetails")]
        public virtual TbDelivery DeliveryNavigation { get; set; } = null!;

        [ForeignKey("Sale")]
        [InverseProperty("TbDeliveryDetails")]
        public virtual TbSale SaleNavigation { get; set; } = null!;

        [ForeignKey("Status")]
        [InverseProperty("TbDeliveryDetails")]
        public virtual TbDeliveryDetailsStatus StatusNavigation { get; set; } = null!;
    }
}
