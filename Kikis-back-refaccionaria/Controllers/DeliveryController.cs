using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Kikis_back_refaccionaria.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase {

        private readonly IServiceDelivery _service;
        public DeliveryController(IServiceDelivery service) {
            _service = service;
        }

        /*
         *  GET
         */
        [Route("track/")]
        [HttpGet]
        public async Task<IActionResult> GetTracks([FromQuery] TrackFilter filter) {

            var data = await _service.GetTracks(filter);
            var response = new ApiResponse<PagedResponse<TrackRES>>(data);
            return Ok(response);
        }
        [Route("details/")]
        [HttpGet]
        public async Task<IActionResult> GetDeliveryDetails([FromQuery] DeliveryDetailsFilter filter) {

            var data = await _service.GetDeliveryDetails(filter);
            var response = new ApiResponse<PagedResponse<DeliveryDetailRES>>(data);
            return Ok(response);
        }

        /*
         *  DELETE
         */
        [HttpDelete("track/{id}")]
        public async Task<IActionResult> DeleteTrack(int id) {

            var data = await _service.DeleteTrack(id);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }


        /*
         *  POST
         */
        [Route("track/")]
        [HttpPost]
        public async Task<IActionResult> PostTrack(TrackREQ request) {

            var data = await _service.PostTrack(request);
            var response = new ApiResponse<TrackRES>(data);
            return Ok(response);
        }
        [Route("details/")]
        [HttpPost]
        public async Task<IActionResult> PostDeliveryDetails([FromBody] DeliveryDetailREQ request) {

            var data = await _service.PostDeliveryDetail(request);
            var response = new ApiResponse<DeliveryDetailRES>(data);
            return Ok(response);
        }

        /*
         *  PUT
         */
        [Route("track/")]
        [HttpPut]
        public async Task<IActionResult> PutTrack([FromBody] TrackREQ request) {

            var data = await _service.PutTrack(request);
            var response = new ApiResponse<TrackRES>(data);
            return Ok(response);
        }
        [Route("details/")]
        [HttpPut]
        public async Task<IActionResult> PutDeliveryDetails([FromBody] DeliveryDetailREQ request) {

            var data = await _service.PutDeliveryDetail(request);
            var response = new ApiResponse<DeliveryDetailRES>(data);
            return Ok(response);
        }
        [Route("track/start/{id}")]
        [HttpPut]
        public async Task<IActionResult> PutTrackStart(int id) {

            var data = await _service.PutTrackStart(id);
            var response = new ApiResponse<TrackRES>(data);
            return Ok(response);
        }
        [Route("track/cancel/{id}")]
        [HttpPut]
        public async Task<IActionResult> PutTrackCancel(int id) {

            var data = await _service.PutTrackCancel(id);
            var response = new ApiResponse<TrackRES>(data);
            return Ok(response);
        }
        [Route("track/finish/{id}")]
        [HttpPut]
        public async Task<IActionResult> PutTrackFinish(int id) {

            var data = await _service.PutTrackFinish(id);
            var response = new ApiResponse<TrackRES>(data);
            return Ok(response);
        }
        [Route("deliver/")]
        [HttpPut]
        public async Task<IActionResult> PutDeliverDelivery([FromQuery] DeliverDeliveryREQ request) {

            var data = await _service.PutDeliverDelivery(request);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }
    }
}
