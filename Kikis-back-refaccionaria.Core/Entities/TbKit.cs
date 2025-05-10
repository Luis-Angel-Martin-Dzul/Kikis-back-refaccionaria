namespace Kikis_back_refaccionaria.Core.Entities {
    public partial class TbKit
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public virtual ICollection<TbProductKit> TbProductKits { get; set; } = new List<TbProductKit>();
    }
}
