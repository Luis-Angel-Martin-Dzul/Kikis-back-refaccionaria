using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbrol")]
    public partial class TbRol {
        [Key]
        public int Id {
            get; set;
        }

        [StringLength(75)]
        public string Name { get; set; } = null!;

        [StringLength(255)]
        public string? Description {
            get; set;
        }

        [InverseProperty("RolNavigation")]
        public virtual ICollection<TbPermission> TbPermissions { get; set; } = new List<TbPermission>();

        [InverseProperty("RolNavigation")]
        public virtual ICollection<TbUser> TbUsers { get; set; } = new List<TbUser>();
    }
}