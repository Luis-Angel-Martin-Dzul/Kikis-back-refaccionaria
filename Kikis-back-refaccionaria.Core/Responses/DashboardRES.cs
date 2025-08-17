namespace Kikis_back_refaccionaria.Core.Responses {
    public class GeneralSummary {

        public SaleSummary Sales { get; set; }

        public List<DailySalesSummary> DailySales { get; set; } = new List<DailySalesSummary>();

        public List<SellerSummary> Sellers { get; set; } = new List<SellerSummary>();

        public List<SellerSummary> TopSellers { get; set; } = new List<SellerSummary>();
    }


    // Métricas generales de ventas
    public class SaleSummary {


        //sale
        public int QuantitySales { get; set; }
        public decimal SalesSubtotal { get; set; }
        public decimal SalesIVA { get; set; }
        public decimal SalesTotal { get; set; }
        public decimal SalesCambio { get; set; }
        public decimal AverageTicket { get; set; }
        public decimal AveragePayment { get; set; }

        //factura
        public int InvoiceCount { get; set; }
        public decimal InvoicedIVA { get; set; }
        public decimal InvoicedTotal { get; set; }

        public double InvoicedPercentage { get; set; }
        public double NotInvoicedPercentage { get; set; }
    }
    public class DailySalesSummary {

        public int Day { get; set; }
        public int SalesNumber { get; set; }
    }
    public class SellerSummary {

        public int Id { get; set;}
        public string? Name { get; set; }
        public int SalesNumber { get; set;}
        public decimal SalesTotal {get; set;}
        public decimal AverageTicket { get; set; }
    }

}





