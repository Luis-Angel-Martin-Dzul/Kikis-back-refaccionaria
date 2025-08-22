using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kikis_back_refaccionaria.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase {

        private readonly IServiceDashboard _service;
        public DashboardController(IServiceDashboard service) {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard([FromQuery] SaleFilter filter) {

            var data = await _service.getSales(filter);

            return Ok(data);
        }

    }
}
