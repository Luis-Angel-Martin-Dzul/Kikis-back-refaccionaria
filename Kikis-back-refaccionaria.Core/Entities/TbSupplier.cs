using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbsupplier")]
    public partial class TbSupplier {
        [Key]
        public int Id {
            get; set;
        }

        [StringLength(100)]
        public string BusinessName { get; set; } = null!;

        [StringLength(100)]
        public string TradeName { get; set; } = null!;

        [StringLength(13)]
        public string RFC { get; set; } = null!;

        [StringLength(40)]
        public string CURP { get; set; } = null!;

        [StringLength(50)]
        public string Email { get; set; } = null!;

        [StringLength(20)]
        public string Cellphone { get; set; } = null!;

        [StringLength(20)]
        public string? Cellphone2 {
            get; set;
        }

        [StringLength(200)]
        public string Address { get; set; } = null!;

        [StringLength(100)]
        public string Owner { get; set; } = null!;

        [StringLength(150)]
        public string? Representative {
            get; set;
        }

        public bool IsActive {
            get; set;
        }

        [InverseProperty("SupplierNavigation")]
        public virtual ICollection<TbProductSupplier> TbProductSuppliers { get; set; } = new List<TbProductSupplier>();
    }
}