using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbdelivery")]
    [Index("User", Name = "tbdelivery_ibfk_1_idx")]
    [Index("Status", Name = "tbdelivery_ibfk_2_idx")]
    public partial class TbDelivery {

        [Key]
        public int Id { get; set; }

        public int User { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }

        public int Status { get; set; }

        public sbyte IsActive { get; set; }

        [ForeignKey("Status")]
        [InverseProperty("TbDeliveries")]
        public virtual TbDeliveryStatus StatusNavigation { get; set; } = null!;

        [ForeignKey("User")]
        [InverseProperty("TbDeliveries")]
        public virtual TbUser UserNavigation { get; set; } = null!;

        [InverseProperty("DeliveryNavigation")]
        public virtual ICollection<TbDeliveryDetail> TbDeliveryDetails { get; set; } = new List<TbDeliveryDetail>();
    }
}
