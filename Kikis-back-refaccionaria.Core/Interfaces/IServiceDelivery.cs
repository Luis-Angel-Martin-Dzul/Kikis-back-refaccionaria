using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;

namespace Kikis_back_refaccionaria.Core.Interfaces {
    public interface IServiceDelivery {

        /*
         *  GET
         */
        Task<PagedResponse<DeliveryDetailRES>> GetDeliveryDetails(DeliveryDetailsFilter filter);
        Task<PagedResponse<TrackRES>> GetTracks(TrackFilter filter);


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
        Task<TrackRES> PutTrackStart(int id);
        Task<TrackRES> PutTrackCancel(int id);
        Task<TrackRES> PutTrackFinish(int id);
        Task<bool> PutDeliverDelivery(DeliverDeliveryREQ request);
    }
}
