using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Models;

namespace MSPremiumProject.Controllers
{
    public class LocalityController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LocalityController> _logger;

        public LocalityController(AppDbContext context, ILogger<LocalityController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ... outras actions (Index, Details, Edit, Delete) ...

        // GET: Localidades/Create
        public async Task<IActionResult> Create()
        {
            // Popula o ViewBag.PaisesList com a lista de países para o dropdown
            // Ordenado por NomePais para melhor usabilidade
            ViewBag.PaisesList = new SelectList(await _context.Paises.OrderBy(p => p.NomePais).ToListAsync(), "PaisId", "NomePais");
            return View(new Localidade()); // Passa um novo objeto Localidade para o formulário
        }

        // POST: Localidades/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NomeLocalidade,Regiao,PaisId")] Localidade localidade) // Adicione LocalidadeId se não for auto-incremento
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(localidade);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Localidade '{localidade.NomeLocalidade}' criada com sucesso.");
                    TempData["MensagemSucesso"] = "Localidade adicionada com sucesso!";
                    return RedirectToAction(nameof(Index)); // Ou para onde você quiser redirecionar após sucesso
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro ao criar localidade.");
                // Adicionar uma mensagem de erro mais específica se possível, ex: verificar se já existe
                ModelState.AddModelError(string.Empty, "Não foi possível guardar a localidade. Tente novamente, e se o problema persistir, contacte o suporte.");
                TempData["MensagemErro"] = "Erro ao adicionar localidade.";
            }

            // Se ModelState não for válido ou ocorrer um erro, repopula o dropdown de países e retorna à view
            ViewBag.PaisesList = new SelectList(await _context.Paises.OrderBy(p => p.NomePais).ToListAsync(), "PaisId", "NomePais", localidade.PaisId);
            return View(localidade);
        }

        public IActionResult InserirLocalidade()
        {
            return View("~/Views/Locality/CreateLocality.cshtml");
        }
    }
}
