namespace Kikis_back_refaccionaria.Core.Responses {
    public class DeliveryDetailRES {

        public int Id { get; set; }

        public int? Delivery { get; set; }

        public int Sale { get; set; }

        public string Responsible { get; set; } = null!;

        public string Address { get; set; } = null!;

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public GenericCatalog Status { get; set; }

        public string Comments { get; set; } = null!;

        public DateTime CreateDate { get; set; }

    }
}
