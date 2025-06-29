using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;

namespace Kikis_back_refaccionaria.Core.Interfaces {
    public interface IServiceDelivery {

        /*
         *  GET
         */
        Task<IEnumerable<DeliveryDetailRES>> GetDeliveryDetails(DeliveryDetailsFilter filter);
        Task<IEnumerable<TrackRES>> GetTracks(TrackFilter filter);


        /*
         *  DELETE
         */
        Task<bool> DeleteTrack(int id);


        /*
         *  POST
         */
        Task<TrackRES> PostTrack(TrackREQ request);
        Task<DeliveryDetailRES> PostDeliveryDetail(DeliveryDetailREQ request);


        /*
         *  PUT
         */
        Task<TrackRES> PutTrack(TrackREQ request);
        Task<DeliveryDetailRES> PutDeliveryDetail(DeliveryDetailREQ request);
    }
}
