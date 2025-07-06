using Kikis_back_refaccionaria.Core.Filters;
using Kikis_back_refaccionaria.Core.Interfaces;
using Kikis_back_refaccionaria.Core.Request;
using Kikis_back_refaccionaria.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Kikis_back_refaccionaria.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase {

        private readonly IServiceUser _service;
        public UserController(IServiceUser service) {
            _service = service;
        }


        /*
         *  GET
         */
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserFilter filter) {

            var data = await _service.GetUsers(filter);
            var response = new ApiResponse<PagedResponse<UserRES>>(data);
            return Ok(response);
        }
        [Route("rols/")]
        [HttpGet]
        public async Task<IActionResult> GetRols([FromQuery] RolFilter filter) {

            var data = await _service.GetRols(filter);
            var response = new ApiResponse<PagedResponse<RolRES>>(data);
            return Ok(response);
        }


        /*
         *  DELETE
         */
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id) {

            var data = await _service.DeleteUser(id);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }


        /*
         *  POST
         */
        [HttpPost]
        public async Task<IActionResult> PostUser(UserREQ request) {

            var data = await _service.PostUser(request);
            var response = new ApiResponse<UserRES>(data);
            return Ok(response);
        }
        [Route("auth/")]
        [HttpPost]
        public async Task<IActionResult> Login(AuthREQ request) {

            var data = await _service.Login(request);
            var response = new ApiResponse<UserAuthRES>(data);
            return Ok(response);
        }
        [Route("rols/")]
        [HttpPost]
        public async Task<IActionResult> PostRols(RolRES request) {

            var data = await _service.PostRols(request);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }


        /*
         *  PUT
         */
        [HttpPut]
        public async Task<IActionResult> PutUser(UserREQ request) {

            var data = await _service.PutUser(request);
            var response = new ApiResponse<UserRES>(data);
            return Ok(response);
        }
        [Route("rols/")]
        [HttpPut]
        public async Task<IActionResult> PutRols(RolRES request) {

            var data = await _service.PutRols(request);
            var response = new ApiResponse<bool>(data);
            return Ok(response);
        }


    }
}
