using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Kikis_back_refaccionaria.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase {

        private readonly IServiceClient _service;
        public ClientController(IServiceClient service) {
            _service = service;
        }


        /*
         *  GET
         */
        [HttpGet]
        public async Task<IActionResult> GetClients([FromQuery] ClientFilter filter) {

            var data = await _service.GetClients(filter);
            var response = new ApiResponse<IEnumerable<ClientRES>>(data);
            return Ok(response);
        }


        /*
         *  DELETE
         */
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id) {

            var data = await _service.DeleteClient(id);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }


        /*
         *  POST
         */
        [HttpPost]
        public async Task<IActionResult> PostClient(ClientREQ request) {

            var data = await _service.PostClient(request);
            var response = new ApiResponse<ClientRES>(data);
            return Ok(response);
        }


        /*
         *  PUT
         */
        [HttpPut]
        public async Task<IActionResult> PutClient(ClientREQ request) {

            var data = await _service.PutClient(request);
            var response = new ApiResponse<ClientRES>(data);
            return Ok(response);
        }
    }
}
