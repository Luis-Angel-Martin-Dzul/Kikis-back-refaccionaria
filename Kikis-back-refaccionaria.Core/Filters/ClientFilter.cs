namespace Kikis_back_refaccionaria.Core.Filters {
    public class ClientFilter : PaginationFilter {

        public int? Id { get; set; }
        public string? Name { get; set; }
    }
}
