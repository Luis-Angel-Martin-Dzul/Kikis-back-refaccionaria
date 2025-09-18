using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbtransactionhistory")]
    public partial class TbTransactionHistory {

        [Key]
        public int Id { get; set; }

        public int? User { get; set; }

        [StringLength(50)]
        public string? Path { get; set; }

        [StringLength(20)]
        public string? Method { get; set; }

        [StringLength(15)]
        public string? IPAddress { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? Date { get; set; }

        [Column(TypeName = "text")]
        public string? RequestBody { get; set; }

        [Column(TypeName = "text")]
        public string? ResponseBody { get; set; }

        [StringLength(3)]
        public string? ResponseStatus { get; set; }
    }
}
