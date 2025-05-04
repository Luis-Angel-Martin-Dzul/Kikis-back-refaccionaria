namespace Kikis_back_refaccionaria.Core.Entities {
    public partial class TbSale{

        public int Id { get; set; }

        public int Seller { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Iva { get; set; }

        public decimal Total { get; set; }

        public decimal Pay { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual TbUser SellerNavigation { get; set; } = null!;

        public virtual ICollection<TbSaleDetail> TbSaleDetails { get; set; } = new List<TbSaleDetail>();
    }
}
