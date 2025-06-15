using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbtrackstatus")]
    public partial class TbTrackStatus {

        [Key]
        public int Id { get; set; }

        [StringLength(75)]
        public string Name { get; set; } = null!;

        [StringLength(225)]
        public string? Description { get; set; }

        [InverseProperty("StatusNavigation")]
        public virtual ICollection<TbTrack> TbTracks { get; set; } = new List<TbTrack>();
    }
}
