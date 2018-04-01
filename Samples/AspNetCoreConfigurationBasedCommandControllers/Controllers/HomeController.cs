using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreConfigurationBasedCommandControllers.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("/swagger");
        }
    }
}
