using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Kikis_back_refaccionaria.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ToolController : ControllerBase {

        private readonly IService _service;
        public ToolController(IService service) {
            _service = service;
        }


        /*
         *  GET
         */
        [HttpGet]
        public async Task<IActionResult> GetTools([FromQuery] ToolFilter filter) {

            var data = await _service.GetTools(filter);
            var response = new ApiResponse<IEnumerable<ToolRES>>(data);
            return Ok(response);
        }


        /*
         *  DELETE
         */
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTool(int id) {

            var data = await _service.DeleteTool(id);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }


        /*
         *  POST
         */
        [HttpPost]
        public async Task<IActionResult> PostTool([FromBody] ToolREQ request) {

            var data = await _service.PostTool(request);
            var response = new ApiResponse<ToolRES>(data);
            return Ok(response);
        }


        /*
         *  PUT
         */
        [Route("promo/")]
        [HttpPut]
        public async Task<IActionResult> PutToolPromotion(ToolPromotionREQ request) {

            var data = await _service.PutToolPromotion(request);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }
        [Route("stock/")]
        [HttpPut]
        public async Task<IActionResult> PutToolStock(ToolStockREQ request) {

            var data = await _service.PutToolStock(request);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }

    }
}
