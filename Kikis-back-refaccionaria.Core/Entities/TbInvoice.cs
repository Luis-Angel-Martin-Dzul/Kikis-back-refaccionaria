using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbinvoices")]
    [Index("Sale", Name = "tbinvoices_ibfk_1_idx")]
    public partial class TbInvoice {

        [Key]
        public int Id { get; set; }

        public int Sale { get; set; }

        [StringLength(75)]
        public string Name { get; set; } = null!;

        [StringLength(13)]
        public string RFC { get; set; } = null!;

        [StringLength(5)]
        public string TaxRegime { get; set; } = null!;

        [StringLength(100)]
        public string TaxRegimeName { get; set; } = null!;

        [StringLength(5)]
        public string CodePostal { get; set; } = null!;

        [StringLength(5)]
        public string UseCFDI { get; set; } = null!;

        [StringLength(100)]
        public string UseCFDIName { get; set; } = null!;

        [StringLength(100)]
        public string Contact { get; set; } = null!;
        
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }

        [ForeignKey("Sale")]
        [InverseProperty("TbInvoices")]
        public virtual TbSale SaleNavigation { get; set; } = null!;
    }
}
