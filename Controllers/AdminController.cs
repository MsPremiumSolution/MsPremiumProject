using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MSPremiumProject.Controllers
{
    public class AdminController : Controller
    {
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Comercial")]
        public IActionResult adminMenu()
        {
            return View("~/Views/Admin/AdminMenu.cshtml");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateRole()
        {
            return View("~/Views/Admin/CreateRole.cshtml");
        }



    }
}
