using Microsoft.AspNetCore.Mvc;
using MSPremiumProject.Data;
using MSPremiumProject.Models;

namespace MSPremiumProject.Controllers
{
    public class RolesController : Controller
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Admin/CreateRole.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Role role)
        {
            if (ModelState.IsValid)
            {
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();

                // Mostra mensagem (opcional)
                TempData["Success"] = "Role criado com sucesso!";

                // Redirect para a mesma view limpa
                return RedirectToAction("Create");
            }

            // Se houver erros, volta à view com os dados preenchidos
            return View("~/Views/Admin/CreateRole.cshtml");
        }
    }
}
