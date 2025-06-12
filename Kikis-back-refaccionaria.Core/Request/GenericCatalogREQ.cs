namespace Kikis_back_refaccionaria.Core.Request {
    public class GenericCatalogREQ {

        public int? Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
