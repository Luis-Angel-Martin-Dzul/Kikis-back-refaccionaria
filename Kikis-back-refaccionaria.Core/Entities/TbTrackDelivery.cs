using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbtrackdelivery")]
    [Index("Track", Name = "tbtrackdelivery_ibfk_1_idx")]
    [Index("Delivery", Name = "tbtrackdelivery_ibfk_2_idx")]
    public partial class TbTrackDelivery {

        [Key]
        public int Id { get; set; }

        public int Track { get; set; }

        public int Delivery { get; set; }

        [ForeignKey("Delivery")]
        [InverseProperty("TbTrackDeliveries")]
        public virtual TbDeliveryDetail DeliveryNavigation { get; set; } = null!;

        [ForeignKey("Track")]
        [InverseProperty("TbTrackDeliveries")]
        public virtual TbTrack TrackNavigation { get; set; } = null!;
    }
}
