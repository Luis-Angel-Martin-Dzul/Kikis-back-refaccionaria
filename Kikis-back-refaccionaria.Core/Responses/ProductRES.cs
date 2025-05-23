﻿namespace Kikis_back_refaccionaria.Core.Responses {
    public class ProductRES {

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Barcode { get; set; } = null!;

        public string Brand { get; set; }

        public string Category { get; set; }

        public GenericCatalog Hallway { get; set; }

        public GenericCatalog Shelf { get; set; }

        public GenericCatalog Level { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal? Discount { get; set; }

        public string? Path { get; set; }

        public bool IsActive { get; set; }

        public List<SupplierRES> Suppliers { get; set; }
        public List<GenericCatalog> Kits { get; set; }
    }
}