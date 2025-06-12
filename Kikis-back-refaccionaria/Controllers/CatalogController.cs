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

        /*
         *  GET
         */
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


        /*
         *  POST
         */
        [Route("category/")]
        [HttpPost]
        public async Task<IActionResult> PostProductCategory([FromBody] GenericCatalogREQ request) {

            var data = await _service.PostProductCategory(request);
            var response = new ApiResponse<GenericCatalog>(data);
            return Ok(response);
        }
        [Route("brand/")]
        [HttpPost]
        public async Task<IActionResult> PostProductBrand([FromBody] GenericCatalogREQ request) {

            var data = await _service.PostProductBrand(request);
            var response = new ApiResponse<GenericCatalog>(data);
            return Ok(response);
        }
        [Route("kit/")]
        [HttpPost]
        public async Task<IActionResult> PostProductKit([FromBody] GenericCatalogREQ request) {

            var data = await _service.PostProductKit(request);
            var response = new ApiResponse<GenericCatalog>(data);
            return Ok(response);
        }
        [Route("hallway/")]
        [HttpPost]
        public async Task<IActionResult> PostProductHallway([FromBody] GenericCatalogREQ request) {

            var data = await _service.PostProductHallway(request);
            var response = new ApiResponse<GenericCatalog>(data);
            return Ok(response);
        }
        [Route("level/")]
        [HttpPost]
        public async Task<IActionResult> PostProductLevel([FromBody] GenericCatalogREQ request) {

            var data = await _service.PostProductLevel(request);
            var response = new ApiResponse<GenericCatalog>(data);
            return Ok(response);
        }
        [Route("shelf/")]
        [HttpPost]
        public async Task<IActionResult> PostProductShelf([FromBody] GenericCatalogREQ request) {

            var data = await _service.PostProductShelf(request);
            var response = new ApiResponse<GenericCatalog>(data);
            return Ok(response);
        }

        /*
         *  PUT
         */
        [Route("category/")]
        [HttpPut]
        public async Task<IActionResult> PutProductCategory([FromBody] GenericCatalogREQ request) {

            var data = await _service.PutProductCategory(request);
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
        [Route("kit/")]
        [HttpPut]
        public async Task<IActionResult> PutProductKit([FromBody] GenericCatalogREQ request) {

            var data = await _service.PutProductKit(request);
            var response = new ApiResponse<GenericCatalog>(data);
            return Ok(response);
        }
        [Route("hallway/")]
        [HttpPut]
        public async Task<IActionResult> PutProductHallway([FromBody] GenericCatalogREQ request) {

            var data = await _service.PutProductHallway(request);
            var response = new ApiResponse<GenericCatalog>(data);
            return Ok(response);
        }
        [Route("level/")]
        [HttpPut]
        public async Task<IActionResult> PutProductLevel([FromBody] GenericCatalogREQ request) {

            var data = await _service.PutProductLevel(request);
            var response = new ApiResponse<GenericCatalog>(data);
            return Ok(response);
        }
        [Route("shelf/")]
        [HttpPut]
        public async Task<IActionResult> PutProductShelf([FromBody] GenericCatalogREQ request) {

            var data = await _service.PutProductShelf(request);
            var response = new ApiResponse<GenericCatalog>(data);
            return Ok(response);
        }
    }
}
