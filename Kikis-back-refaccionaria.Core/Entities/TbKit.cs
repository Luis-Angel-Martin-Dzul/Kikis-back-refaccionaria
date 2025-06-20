using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbkit")]
    public partial class TbKit {
        [Key]
        public int Id { get; set; }

        [StringLength(75)]
        public string Name { get; set; } = null!;

        [StringLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        [InverseProperty("KitNavigation")]
        public virtual ICollection<TbProductKit> TbProductKits { get; set; } = new List<TbProductKit>();
    }
}
