// Ficheiro: Controllers/CountryController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Models;
using Microsoft.Extensions.Logging;
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

        // GET: /Country ou /Country/Index
        // Esta ação mostra a lista de todos os países.
        public async Task<IActionResult> Index()
        {
            var paises = await _context.Paises
                                       .OrderBy(p => p.NomePais)
                                       .ToListAsync();
            return View(paises); // Precisa de uma View chamada Views/Country/Index.cshtml
        }

        // GET: /Country/Create
        // Esta ação mostra o formulário para criar um novo país.
        public IActionResult Create()
        {
            // O nome do ficheiro da view deve ser Views/Country/Create.cshtml
            return View("CreateCountry", new Pai());
        }

        // POST: /Country/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NomePais, CodigoIso")] Pai pais) // Adicionado CodigoIso ao Bind
        {
            if (ModelState.IsValid)
            {
                // Verifica se já existe um país com o mesmo nome
                if (await _context.Paises.AnyAsync(p => p.NomePais.ToLower() == pais.NomePais.ToLower()))
                {
                    ModelState.AddModelError("NomePais", "Já existe um país com este nome.");
                    return View("CreateCountry", pais); // Retorna para o formulário com o erro
                }

                try
                {
                    _context.Add(pais);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"País '{pais.NomePais}' criado com sucesso com ID: {pais.PaisId}.");
                    TempData["MensagemSucesso"] = $"País '{pais.NomePais}' adicionado com sucesso!";
                    return RedirectToAction(nameof(Index)); // Redireciona para a lista de países
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, $"Erro ao criar país '{pais.NomePais}'.");
                    ModelState.AddModelError(string.Empty, "Não foi possível guardar o país. Tente novamente.");
                    TempData["MensagemErro"] = "Erro ao adicionar o país.";
                }
            }

            // Se o modelo não for válido, retorna à view com os erros
            return View("CreateCountry", pais);
        }
    }
}