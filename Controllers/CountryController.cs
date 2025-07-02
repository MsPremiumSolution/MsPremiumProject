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
        // Ação para mostrar a lista de todos os países
        public async Task<IActionResult> Index()
        {
            // Busca os países ordenados por nome e passa para a View "Index.cshtml"
            var paises = await _context.Paises.OrderBy(p => p.NomePais).ToListAsync();
            return View(paises);
        }

        // GET: /Country/Create
        // Ação para mostrar o formulário de criação
        public IActionResult Create()
        {
            // Retorna a View "Create.cshtml" com um novo objeto "Pai" vazio
            return View(new Pai());
        }

        // POST: /Country/Create
        // Ação para processar os dados do formulário
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NomePais, CodigoIso")] Pai pais) // Garanta que o modelo Pai tem CodigoIso
        {
            if (ModelState.IsValid)
            {
                // Verifica se já existe um país com o mesmo nome (ignora maiúsculas/minúsculas)
                bool nomeJaExiste = await _context.Paises.AnyAsync(p => p.NomePais.ToLower() == pais.NomePais.ToLower());
                if (nomeJaExiste)
                {
                    ModelState.AddModelError("NomePais", "Já existe um país com este nome.");
                }

                // Se ainda for válido (não encontrou nome duplicado)
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Add(pais);
                        await _context.SaveChangesAsync();
                        TempData["MensagemSucesso"] = $"País '{pais.NomePais}' adicionado com sucesso!";
                        return RedirectToAction(nameof(Index)); // Redireciona para a lista de países
                    }
                    catch (DbUpdateException ex)
                    {
                        _logger.LogError(ex, $"Erro ao criar o país '{pais.NomePais}'.");
                        ModelState.AddModelError(string.Empty, "Ocorreu um erro ao guardar os dados. Tente novamente.");
                    }
                }
            }

            // Se o modelo não for válido ou se ocorreu um erro, retorna à mesma view com os dados preenchidos
            return View(pais);
        }
    }
}