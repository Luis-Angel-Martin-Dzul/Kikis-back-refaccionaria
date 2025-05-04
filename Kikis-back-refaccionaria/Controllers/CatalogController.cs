using Kikis_back_refaccionaria.Core.Interfaces;
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
        public async Task<IActionResult> GetToolCategory() {

            var data = await _service.GetToolCategory();
            var response = new ApiResponse<IEnumerable<GenericCatalog>>(data);
            return Ok(response);
        }

        [Route("brand/")]
        [HttpGet]
        public async Task<IActionResult> GetToolBrand() {

            var data = await _service.GetToolBrand();
            var response = new ApiResponse<IEnumerable<GenericCatalog>>(data);
            return Ok(response);
        }
    }
}
