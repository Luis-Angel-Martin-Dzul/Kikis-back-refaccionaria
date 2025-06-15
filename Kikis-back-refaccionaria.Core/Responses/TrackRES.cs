using Kikis_back_refaccionaria.Core.Request;

namespace Kikis_back_refaccionaria.Core.Responses {
    public class TrackRES {

        public int Id { get; set; }

        public string Name { get; set; }

        public UserREQ User { get; set; }

        public DateTime CreateDate { get; set; }

        public GenericCatalog Status { get; set; }

        public bool IsActive { get; set; }

        public List<DeliveryDetailRES>? Deliveries { get; set; }
    }
}
