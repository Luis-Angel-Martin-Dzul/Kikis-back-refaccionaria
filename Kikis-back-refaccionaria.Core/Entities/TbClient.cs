using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbclient")]
    public partial class TbClient {

        [Key]
        public int Id { get; set; }

        [StringLength(45)]
        public string FirstName { get; set; } = null!;

        [StringLength(45)]
        public string LastName { get; set; } = null!;

        [StringLength(15)]
        public string Cellphone { get; set; } = null!;

        [StringLength(50)]
        public string Email { get; set; } = null!;

        [StringLength(100)]
        public string Address { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
