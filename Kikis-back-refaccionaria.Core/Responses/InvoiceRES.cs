namespace Kikis_back_refaccionaria.Core.Responses {
    public class InvoiceRES {

        public int Id { get; set; }

        public int Sale { get; set; }

        public string Name { get; set; } = null!;

        public string RFC { get; set; } = null!;

        public string TaxRegime { get; set; } = null!;

        public string TaxRegimeName { get; set; } = null!;

        public string CodePostal { get; set; } = null!;

        public string UseCFDI { get; set; } = null!;

        public string UseCFDIName { get; set; } = null!;

        public string Contact { get; set; } = null!;
        
        public DateTime CreateDate { get; set; }
    }
}
