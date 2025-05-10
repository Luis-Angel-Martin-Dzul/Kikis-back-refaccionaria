using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Kikis_back_refaccionaria.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase {

        private readonly IService _service;
        public ProductController(IService service) {
            _service = service;
        }


        /*
         *  GET
         */
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductFilter filter) {

            var data = await _service.GetProducts(filter);
            var response = new ApiResponse<IEnumerable<ProductRES>>(data);
            return Ok(response);
        }


        /*
         *  DELETE
         */
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id) {

            var data = await _service.DeleteProduct(id);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }


        /*
         *  POST
         */
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] ProductREQ request) {

            var data = await _service.PostProduct(request);
            var response = new ApiResponse<ProductRES>(data);
            return Ok(response);
        }


        /*
         *  PUT
         */
        [Route("promo/")]
        [HttpPut]
        public async Task<IActionResult> PutProductPromotion(ProductPromotionREQ request) {

            var data = await _service.PutProductPromotion(request);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }
        [Route("stock/")]
        [HttpPut]
        public async Task<IActionResult> PutProductStock(ProductStockREQ request) {

            var data = await _service.PutProductStock(request);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }
        [Route("warehouse/")]
        [HttpPut]
        public async Task<IActionResult> PutProductWarehouse(ProductWarehouseREQ request) {

            var data = await _service.PutProductWarehouse(request);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }
        [Route("supplier/")]
        [HttpPut]
        public async Task<IActionResult> PutProductSupplier(ProductRES request) {

            var data = await _service.PutProductSupplier(request);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }
        [Route("kit/")]
        [HttpPut]
        public async Task<IActionResult> PutProductKit(ProductRES request) {

            var data = await _service.PutProductKit(request);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }

    }
}
