using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbpermission")]
    [Index("Module", Name = "Module")]
    [Index("Rol", Name = "Rol")]
    public partial class TbPermission {
        [Key]
        public int Id {
            get; set;
        }

        public int Rol {
            get; set;
        }

        public int Module {
            get; set;
        }

        public bool CanAdd {
            get; set;
        }

        public bool CanEdit {
            get; set;
        }

        public bool CanDelete {
            get; set;
        }

        public bool CanView {
            get; set;
        }

        [ForeignKey("Module")]
        [InverseProperty("TbPermissions")]
        public virtual tbmodule ModuleNavigation { get; set; } = null!;

        [ForeignKey("Rol")]
        [InverseProperty("TbPermissions")]
        public virtual TbRol RolNavigation { get; set; } = null!;
    }
}