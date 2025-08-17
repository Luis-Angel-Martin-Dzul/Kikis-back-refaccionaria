namespace Kikis_back_refaccionaria.Core.Filters {
    public class SaleFilter : PaginationFilter {

        public int? Id { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateFinish { get; set; }
        public bool? IsInvoiced { get; set; }
    }
}
