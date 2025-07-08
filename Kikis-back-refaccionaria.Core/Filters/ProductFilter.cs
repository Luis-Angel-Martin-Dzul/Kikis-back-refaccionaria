namespace Kikis_back_refaccionaria.Core.Filters {
    public class ProductFilter : PaginationFilter {

        public int? Id { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public int? Supplier { get; set; }
        public int? Hallway { get; set; }
        public int? Level { get; set; }
        public int? Shelf { get; set; }
        public int? Kit { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

    }
}
