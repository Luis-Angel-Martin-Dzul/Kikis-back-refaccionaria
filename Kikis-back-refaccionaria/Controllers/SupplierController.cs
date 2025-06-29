using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Kikis_back_refaccionaria.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase {

        private readonly IServiceSupplier _service;
        public SupplierController(IServiceSupplier service) {
            _service = service;
        }


        /*
         *  GET
         */
        [HttpGet]
        public async Task<IActionResult> GetSupplier() {

            var data = await _service.GetSupplier();
            var response = new ApiResponse<IEnumerable<SupplierRES>>(data);
            return Ok(response);
        }


        /*
         *  DELETE
         */
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id) {

            var data = await _service.DeleteSupplier(id);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }


        /*
         *  POST
         */
        [HttpPost]
        public async Task<IActionResult> PostSupplier([FromBody] SupplierRES request) {

            var data = await _service.PostSupplier(request);
            var response = new ApiResponse<SupplierRES>(data);
            return Ok(response);
        }

        /*
         *  PUT
         */
        [HttpPut]
        public async Task<IActionResult> PutSupplier([FromBody] SupplierRES request) {

            var data = await _service.PutSupplier(request);
            var response = new ApiResponse<SupplierRES>(data);
            return Ok(response);
        }
    }
}
