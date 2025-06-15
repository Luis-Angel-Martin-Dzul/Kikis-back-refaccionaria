using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbtrack")]
    public partial class TbTrack {

        [Key]
        public int Id { get; set; }

        public int User { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }

        public int Status { get; set; }

        public bool IsActive { get; set; }

        [ForeignKey("Status")]
        [InverseProperty("TbTracks")]
        public virtual TbTrackStatus StatusNavigation { get; set; } = null!;

        [ForeignKey("User")]
        [InverseProperty("TbTracks")]
        public virtual TbUser UserNavigation { get; set; } = null!;
    }
}
