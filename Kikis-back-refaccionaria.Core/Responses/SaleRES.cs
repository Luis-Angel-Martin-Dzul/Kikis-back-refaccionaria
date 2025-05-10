namespace Kikis_back_refaccionaria.Core.Responses {
    public class SaleRES {

        public int Id { get; set; }

        public GenericCatalog Seller { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Iva { get; set; }

        public decimal Total { get; set; }

        public decimal Pay { get; set; }

        public DateTime CreateDate { get; set; }

        public List<SaleDetail> SaleDetails { get; set; }

    }

    public class SaleDetail
    {
        public int Id { get; set; }

        public int Product { get; set; }

        public decimal Price { get; set; }

        public decimal PriceUnit { get; set; }

        public int Quantity { get; set; }

        public decimal Total { get; set; }
    }
}
