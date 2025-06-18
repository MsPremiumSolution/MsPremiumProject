using Microsoft.AspNetCore.Mvc;

namespace MSPremiumProject.Controllers
{
    public class ClientController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult ClientPainel()
        {
            return View("~/Views/Client/ClientPainel.cshtml");
        }
    }
}
