namespace Kikis_back_refaccionaria.Core.Entities {
    public class TbProductSupplier {

        public int Id { get; set; }

        public int Product { get; set; }

        public int Supplier { get; set; }

        public virtual TbProduct ProductNavigation { get; set; } = null!;

        public virtual TbSupplier SupplierNavigation { get; set; } = null!;
    }
}
