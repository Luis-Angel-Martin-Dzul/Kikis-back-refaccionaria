namespace Kikis_back_refaccionaria.Core.Entities {
    public partial class TbProductKit
    {
        public int Id { get; set; }

        public int Product { get; set; }

        public int Kit { get; set; }

        public virtual TbKit KitNavigation { get; set; } = null!;

        public virtual TbProduct ProductNavigation { get; set; } = null!;
    }
}
