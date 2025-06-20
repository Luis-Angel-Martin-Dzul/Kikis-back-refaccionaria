using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikis_back_refaccionaria.Core.Entities {

    [Table("tbproductshelf")]
    public partial class TbProductShelf {
        [Key]
        public int Id { get; set; }

        [StringLength(75)]
        public string Name { get; set; } = null!;

        [StringLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        [InverseProperty("ShelfNavigation")]
        public virtual ICollection<TbProduct> TbProducts { get; set; } = new List<TbProduct>();
    }
}