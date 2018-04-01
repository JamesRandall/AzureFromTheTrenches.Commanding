using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreConfigurationBasedCommandControllers.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Redirect("/swagger");
        }
    }
}
