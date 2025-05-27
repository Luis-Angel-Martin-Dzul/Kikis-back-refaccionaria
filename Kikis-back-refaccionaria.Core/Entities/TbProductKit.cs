using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbproductkit")]
    [Index("Kit", Name = "Kit")]
    [Index("Product", Name = "Product")]
    public partial class TbProductKit {
        [Key]
        public int Id {
            get; set;
        }

        public int Product {
            get; set;
        }

        public int Kit {
            get; set;
        }

        [ForeignKey("Kit")]
        [InverseProperty("TbProductKits")]
        public virtual TbKit KitNavigation { get; set; } = null!;

        [ForeignKey("Product")]
        [InverseProperty("TbProductKits")]
        public virtual TbProduct ProductNavigation { get; set; } = null!;
    }
}
