namespace Kikis_back_refaccionaria.Core.Filters {
    public class DeliveryDetailsFilter : PaginationFilter {

        public int? Id { get; set; }
        public int? Status { get; set; }

    }
}
