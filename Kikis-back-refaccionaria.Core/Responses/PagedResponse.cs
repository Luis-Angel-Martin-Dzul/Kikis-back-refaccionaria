namespace Kikis_back_refaccionaria.Core.Responses {
    public class PagedResponse<T> {

        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
