namespace Kikis_back_refaccionaria.Core.Entities {

    public partial class TbSaleDetail
    {
        public int Id { get; set; }

        public int Sale { get; set; }

        public int Tool { get; set; }

        public decimal Price { get; set; }

        public decimal PriceUnit { get; set; }

        public int Quantity { get; set; }

        public decimal Total { get; set; }

        public virtual TbSale SaleNavigation { get; set; } = null!;
    }
}
