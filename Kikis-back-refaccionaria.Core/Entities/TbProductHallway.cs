namespace Kikis_back_refaccionaria.Core.Entities {
    public class TbProductHallway {

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public virtual ICollection<TbProduct> TbProducts { get; set; } = new List<TbProduct>();
    }
}
