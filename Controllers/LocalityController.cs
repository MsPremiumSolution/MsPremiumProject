using Microsoft.AspNetCore.Mvc;

namespace MSPremiumProject.Controllers
{
    public class LocalityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult InserirLocalidade()
        {
            return View("~/Views/Locality/CreateLocality.cshtml");
        }
    }
}
