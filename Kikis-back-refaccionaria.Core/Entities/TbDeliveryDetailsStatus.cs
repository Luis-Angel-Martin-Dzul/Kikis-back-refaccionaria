using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbdeliverydetailsstatus")]
    public partial class TbDeliveryDetailsStatus {

        [Key]
        public int Id { get; set; }

        [StringLength(75)]
        public string Name { get; set; } = null!;

        [StringLength(225)]
        public string? Description { get; set; }

        [InverseProperty("StatusNavigation")]
        public virtual ICollection<TbDeliveryDetail> TbDeliveryDetails { get; set; } = new List<TbDeliveryDetail>();
    }
}
