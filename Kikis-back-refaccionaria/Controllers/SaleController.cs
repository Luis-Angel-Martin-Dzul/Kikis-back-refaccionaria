using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Kikis_back_refaccionaria.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase {

        private readonly IService _service;
        public SaleController(IService service) {
            _service = service;
        }


        /*
         *  GET
         */
        [HttpGet]
        public async Task<IActionResult> GetTools([FromQuery] SaleFilter filter) {

            var data = await _service.GetSales(filter);
            var response = new ApiResponse<IEnumerable<SaleRES>>(data);
            return Ok(response);
        }
        [Route("invoice/")]
        [HttpGet]
        public async Task<IActionResult> GetTools([FromQuery] InvoiceFilter filter) {

            var data = await _service.GetInvoices(filter);
            var response = new ApiResponse<IEnumerable<InvoiceRES>>(data);
            return Ok(response);
        }


        /*
         *  DELETE
         */
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteTool(int id) {

        //    var data = await _service.DeleteTool(id);
        //    var response = new ApiResponse<bool>(data);
        //    return Ok(response);
        //}


        /*
         *  POST
         */
        [HttpPost]
        public async Task<IActionResult> PostSales(SaleREQ request) {

            var data = await _service.PostSales(request);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }
        [Route("invoice/")]
        [HttpPost]
        public async Task<IActionResult> PostInvoice(InvoiceREQ request) {

            var data = await _service.PostInvoice(request);
            var response = new ApiResponse<int>(data);
            return Ok(response);
        }
        [Route("invoice/try/")]
        [HttpPost]
        public async Task<IActionResult> PostTryInvoice(InvoiceTryREQ request) {

            var data = await _service.PostTryInvoice(request);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }


        /*
         *  PUT
         */
        //[Route("promo/")]
        //[HttpPut]
        //public async Task<IActionResult> PutToolPromotion(ToolPromotionREQ request) {

        //    var data = await _service.PutToolPromotion(request);
        //    var response = new ApiResponse<bool>(data);
        //    return Ok(response);
        //}
        //[Route("stock/")]
        //[HttpPut]
        //public async Task<IActionResult> PutToolStock(ToolStockREQ request) {

        //    var data = await _service.PutToolStock(request);
        //    var response = new ApiResponse<bool>(data);
        //    return Ok(response);
        //}

    }
}
