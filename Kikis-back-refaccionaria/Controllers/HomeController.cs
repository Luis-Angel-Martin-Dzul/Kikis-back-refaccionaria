using Microsoft.AspNetCore.Mvc;

namespace Kikis_back_refaccionaria.Controllers {
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
