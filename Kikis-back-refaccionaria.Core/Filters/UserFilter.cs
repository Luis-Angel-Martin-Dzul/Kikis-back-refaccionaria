namespace Kikis_back_refaccionaria.Core.Filters {
    public class UserFilter : PaginationFilter {

        public int? Id { get; set; }
        public List<int>? Roles { get; set; }

    }
}
