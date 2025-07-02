// Ficheiro: Controllers/CountryController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Models;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace MSPremiumProject.Controllers
{
    public class CountryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CountryController> _logger;

        public CountryController(AppDbContext context, ILogger<CountryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Country
        public async Task<IActionResult> Index()
        {
            var paises = await _context.Paises.OrderBy(p => p.NomePais).ToListAsync();
            return View(paises);
        }

        // GET: /Country/Create
        public IActionResult Create()
        {
            return View(new Pai());
        }

        // POST: /Country/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // O [Bind] agora só inclui os campos que vêm do formulário.
        public async Task<IActionResult> Create([Bind("NomePais")] Pai pais)
        {
            // Removemos o CodigoIso da validação inicial, pois será calculado.
            ModelState.Remove("CodigoIso");

            if (ModelState.IsValid)
            {
                bool nomeJaExiste = await _context.Paises.AnyAsync(p => p.NomePais.ToLower() == pais.NomePais.ToLower());
                if (nomeJaExiste)
                {
                    ModelState.AddModelError("NomePais", "Já existe um país com este nome.");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        // --- LÓGICA DE CÁLCULO DO CÓDIGO ISO ---
                        // Garante que o nome do país não é nulo e tem pelo menos 2 caracteres.
                        if (!string.IsNullOrEmpty(pais.NomePais) && pais.NomePais.Length >= 2)
                        {
                            pais.CodigoIso = pais.NomePais.Substring(0, 2).ToUpper();
                        }
                        else
                        {
                            // Se o nome for muito curto, define um valor padrão ou lança um erro.
                            // Vamos adicionar um erro de modelo para ser mais claro.
                            ModelState.AddModelError("NomePais", "O nome do país deve ter pelo menos 2 caracteres.");
                            return View(pais);
                        }

                        _context.Add(pais);
                        await _context.SaveChangesAsync();
                        TempData["MensagemSucesso"] = $"País '{pais.NomePais}' adicionado com sucesso!";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateException ex)
                    {
                        _logger.LogError(ex, $"Erro ao criar o país '{pais.NomePais}'.");
                        ModelState.AddModelError(string.Empty, "Ocorreu um erro ao guardar os dados. Tente novamente.");
                    }
                }
            }

            return View(pais);
        }
    }
}