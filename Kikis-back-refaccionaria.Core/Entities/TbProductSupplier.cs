using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbproductsupplier")]
    [Index("Product", Name = "Product")]
    [Index("Supplier", Name = "Supplier")]
    public partial class TbProductSupplier {
        [Key]
        public int Id { get; set; }

        public int Product { get; set; }

        public int Supplier { get; set; }

        [ForeignKey("Product")]
        [InverseProperty("TbProductSuppliers")]
        public virtual TbProduct ProductNavigation { get; set; } = null!;

        [ForeignKey("Supplier")]
        [InverseProperty("TbProductSuppliers")]
        public virtual TbSupplier SupplierNavigation { get; set; } = null!;
    }
}