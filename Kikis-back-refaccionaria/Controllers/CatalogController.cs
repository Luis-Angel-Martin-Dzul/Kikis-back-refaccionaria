using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Kikis_back_refaccionaria.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase {

        private readonly IServiceCatalogs _service;
        public CatalogController(IServiceCatalogs service) {
            _service = service;
        }

        /*
         *  GET
         */
        [Route("category/")]
        [HttpGet]
        public async Task<IActionResult> GetProductCategory([FromQuery] PaginationFilter filter) {

            var data = await _service.GetProductCategory(filter);
            var response = new ApiResponse<PagedResponse<GenericCatalog>>(data);
            return Ok(response);
        }

        [Route("brand/")]
        [HttpGet]
        public async Task<IActionResult> GetProductBrand([FromQuery] PaginationFilter filter) {

            var data = await _service.GetProductBrand(filter);
            var response = new ApiResponse<PagedResponse<GenericCatalog>>(data);
            return Ok(response);
        }

        [Route("hallway/")]
        [HttpGet]
        public async Task<IActionResult> GetProductHallway([FromQuery] PaginationFilter filter) {

            var data = await _service.GetProductHallway(filter);
            var response = new ApiResponse<PagedResponse<GenericCatalog>>(data);
            return Ok(response);
        }

        [Route("level/")]
        [HttpGet]
        public async Task<IActionResult> GetProductLevel([FromQuery] PaginationFilter filter) {

            var data = await _service.GetProductLevel(filter);
            var response = new ApiResponse<PagedResponse<GenericCatalog>>(data);
            return Ok(response);
        }

        [Route("shelf/")]
        [HttpGet]
        public async Task<IActionResult> GetProductShelf([FromQuery] PaginationFilter filter) {

            var data = await _service.GetProductShelf(filter);
            var response = new ApiResponse<PagedResponse<GenericCatalog>>(data);
            return Ok(response);
        }

        [Route("kit/")]
        [HttpGet]
        public async Task<IActionResult> GetProductKit([FromQuery] PaginationFilter filter) {

            var data = await _service.GetProductKit(filter);
            var response = new ApiResponse<PagedResponse<GenericCatalog>>(data);
            return Ok(response);
        }


        /*
         *  DELETE
         */
        [HttpDelete("category/{id}")]
        public async Task<IActionResult> DeleteProductCategory(int id) {

            var data = await _service.DeleteProductCategory(id);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }
        [HttpDelete("brand/{id}")]
        public async Task<IActionResult> DeleteProductBrand(int id) {

            var data = await _service.DeleteProductBrand(id);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }
        [HttpDelete("hallway/{id}")]
        public async Task<IActionResult> DeleteProductHallway(int id) {

            var data = await _service.DeleteProductHallway(id);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }
        [HttpDelete("level/{id}")]
        public async Task<IActionResult> DeleteProductLevel(int id) {

            var data = await _service.DeleteProductLevel(id);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }
        [HttpDelete("shelf/{id}")]
        public async Task<IActionResult> DeleteProductShelf(int id) {

            var data = await _service.DeleteProductShelf(id);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }
        [HttpDelete("kit/{id}")]
        public async Task<IActionResult> DeleteProductKit(int id) {

            var data = await _service.DeleteProductKit(id);
            var response = new ApiResponse<bool>(data);
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
