using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Kikis_back_refaccionaria.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase {

        private readonly IService _service;
        public CatalogController(IService service) {
            _service = service;
        }

        [Route("category/")]
        [HttpGet]
        public async Task<IActionResult> GetProductCategory() {

            var data = await _service.GetProductCategory();
            var response = new ApiResponse<IEnumerable<GenericCatalog>>(data);
            return Ok(response);
        }

        [Route("brand/")]
        [HttpGet]
        public async Task<IActionResult> GetProductBrand() {

            var data = await _service.GetProductBrand();
            var response = new ApiResponse<IEnumerable<GenericCatalog>>(data);
            return Ok(response);
        }

        [Route("hallway/")]
        [HttpGet]
        public async Task<IActionResult> GetProductHallway() {

            var data = await _service.GetProductHallway();
            var response = new ApiResponse<IEnumerable<GenericCatalog>>(data);
            return Ok(response);
        }

        [Route("level/")]
        [HttpGet]
        public async Task<IActionResult> GetProductLevel() {

            var data = await _service.GetProductLevel();
            var response = new ApiResponse<IEnumerable<GenericCatalog>>(data);
            return Ok(response);
        }

        [Route("shelf/")]
        [HttpGet]
        public async Task<IActionResult> GetProductShelf() {

            var data = await _service.GetProductShelf();
            var response = new ApiResponse<IEnumerable<GenericCatalog>>(data);
            return Ok(response);
        }

        [Route("kit/")]
        [HttpGet]
        public async Task<IActionResult> GetProductKit() {

            var data = await _service.GetProductKit();
            var response = new ApiResponse<IEnumerable<GenericCatalog>>(data);
            return Ok(response);
        }


        [Route("brand/")]
        [HttpPost]
        public async Task<IActionResult> PostProductBrand([FromBody] GenericCatalogREQ request) {

            var data = await _service.PostProductBrand(request);
            var response = new ApiResponse<GenericCatalog>(data);
            return Ok(response);
        }

        [Route("brand/")]
        [HttpPut]
        public async Task<IActionResult> PutProductBrand([FromBody] GenericCatalogREQ request) {

            var data = await _service.PutProductBrand(request);
            var response = new ApiResponse<GenericCatalog>(data);
            return Ok(response);
        }
    }
}
