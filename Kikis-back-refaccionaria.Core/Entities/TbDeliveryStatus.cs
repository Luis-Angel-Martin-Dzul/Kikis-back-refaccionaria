using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbdeliverystatus")]
    public partial class TbDeliveryStatus {

        [Key]
        public int Id { get; set; }

        [StringLength(75)]
        public string Name { get; set; } = null!;

        [StringLength(225)]
        public string? Description { get; set; }

        [InverseProperty("StatusNavigation")]
        public virtual ICollection<TbDelivery> TbDeliveries { get; set; } = new List<TbDelivery>();
    }
}
