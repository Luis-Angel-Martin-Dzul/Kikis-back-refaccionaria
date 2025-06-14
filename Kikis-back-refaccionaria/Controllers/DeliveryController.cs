using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Kikis_back_refaccionaria.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase {

        private readonly IService _service;
        public DeliveryController(IService service) {
            _service = service;
        }

        /*
         *  GET
         */
        [Route("details/")]
        [HttpGet]
        public async Task<IActionResult> GetDeliveryDetails([FromQuery] DeliveryDetailsFilter filter) {

            var data = await _service.GetDeliveryDetails(filter);
            var response = new ApiResponse<IEnumerable<DeliveryDetailRES>>(data);
            return Ok(response);
        }

        /*
         *  POST
         */
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
        [Route("details/")]
        [HttpPut]
        public async Task<IActionResult> PutDeliveryDetails([FromBody] DeliveryDetailREQ request) {

            var data = await _service.PutDeliveryDetail(request);
            var response = new ApiResponse<DeliveryDetailRES>(data);
            return Ok(response);
        }
    }
}
