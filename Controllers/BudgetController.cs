using Microsoft.AspNetCore.Mvc;

namespace MSPremiumProject.Controllers
{
    public class BudgetController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
