using Kikis_back_refaccionaria.Core.Responses;

namespace Kikis_back_refaccionaria.Core.Request {
    public class TrackREQ {

        public int? Id { get; set; }

        public string Name { get; set; }

        public int User { get; set; }

        public DateTime CreateDate { get; set; }

        public GenericCatalog? Status { get; set; }

        public bool IsActive { get; set; }

        public List<TrackDeliveryREQ> Deliveries { get; set; }
    }
}
