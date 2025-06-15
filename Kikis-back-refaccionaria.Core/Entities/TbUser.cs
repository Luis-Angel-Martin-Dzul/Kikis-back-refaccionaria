using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbuser")]
    [Index("Rol", Name = "Rol")]
    public partial class TbUser {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [StringLength(100)]
        public string LastName { get; set; } = null!;

        [StringLength(255)]
        public string Email { get; set; } = null!;

        [StringLength(255)]
        public string Password { get; set; } = null!;

        [StringLength(18)]
        public string Curp { get; set; } = null!;

        public int Rol { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }

        public bool IsActive { get; set; }

        [ForeignKey("Rol")]
        [InverseProperty("TbUsers")]
        public virtual TbRol RolNavigation { get; set; } = null!;

        [InverseProperty("SellerNavigation")]
        public virtual ICollection<TbSale> TbSales { get; set; } = new List<TbSale>();

        [InverseProperty("UserNavigation")]
        public virtual ICollection<TbTrack> TbTracks { get; set; } = new List<TbTrack>();
    }
}