namespace Kikis_back_refaccionaria.Core.Responses {
    public class TrackRES {

        public int Id { get; set; }

        public int User { get; set; }

        public DateTime CreateDate { get; set; }

        public GenericCatalog Status { get; set; }

        public bool IsActive { get; set; }

        public DeliveryDetailRES? Deliveries { get; set; }
    }
}
