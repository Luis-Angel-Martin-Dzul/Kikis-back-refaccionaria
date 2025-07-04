﻿using Kikis_back_refaccionaria.Core.Responses;

namespace Kikis_back_refaccionaria.Core.Request {
    public class SaleREQ {

        public ClientREQ? Client { get; set; }

        public GenericCatalog Seller { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Iva { get; set; }

        public decimal Total { get; set; }

        public decimal Pay { get; set; }

        public DateTime CreateDate { get; set; }

        public List<SaleDetailREQ> SaleDetails { get; set; }

    }

    public class SaleDetailREQ
    {
        public string Name { get; set; }

        public int Product { get; set; }

        public decimal Price { get; set; }

        public decimal PriceUnit { get; set; }

        public int Quantity { get; set; }

        public decimal Total { get; set; }
    }
}
