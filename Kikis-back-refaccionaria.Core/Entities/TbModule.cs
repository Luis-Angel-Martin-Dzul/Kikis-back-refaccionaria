using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbmodule")]
    public partial class tbmodule {
        [Key]
        public int Id { get; set; }

        [StringLength(75)]
        public string Name { get; set; } = null!;

        [StringLength(255)]
        public string? Description { get; set; }

        [InverseProperty("ModuleNavigation")]
        public virtual ICollection<TbPermission> TbPermissions { get; set; } = new List<TbPermission>();
    }
}