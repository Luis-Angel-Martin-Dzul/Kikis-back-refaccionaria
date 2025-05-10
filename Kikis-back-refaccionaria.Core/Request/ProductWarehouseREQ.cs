using Kikis_back_refaccionaria.Core.Responses;

namespace Kikis_back_refaccionaria.Core.Request {
    public class ProductWarehouseREQ {

        public int Id { get; set; }

        public GenericCatalog Hallway { get; set; }

        public GenericCatalog Shelf { get; set; }

        public GenericCatalog Level { get; set; }
    }
}
