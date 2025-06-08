using Kikis_back_refaccionaria.Core.Responses;

namespace Kikis_back_refaccionaria.Core.Request {
    public class ProductREQ {

        public int? Id { get; set; }

        public string Name { get; set; } = null!;

        public string Barcode { get; set; } = null!;

        public int Brand { get; set; }

        public int Category { get; set; }

        public int Hallway { get; set; }

        public int Level { get; set; }

        public int Shelf { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal? Discount { get; set; }

        public string? Path { get; set; }
        
        public bool IsActive { get; set; }

        public List<SupplierRES> Suppliers { get; set; }
        public List<GenericCatalog> Kits { get; set; }

        public string? imgB64 { get; set; }
        public string? imgName { get; set; }
    }
}
