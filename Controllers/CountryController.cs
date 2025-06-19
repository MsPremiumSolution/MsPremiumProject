using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Models;

namespace MSPremiumProject.Controllers
{
    public class CountryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        private readonly AppDbContext _context;
        private readonly ILogger<PaisesController> _logger;

        public PaisesController(AppDbContext context, ILogger<PaisesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Paises/Create
        public IActionResult Create()
        {
            return View(new Pai()); // Passa um novo objeto Pai para o formulário
        }

        // POST: Paises/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NomePais")] Pai pais) // Adicione PaisId se não for auto-incremento e precisar ser inserido
        {
            // Remove PaisId da validação do ModelState se for gerado pelo banco
            // ModelState.Remove("PaisId"); // Descomente se PaisId estiver no Bind mas não deve ser validado como input

            // Verifica se já existe um país com o mesmo nome (opcional, mas bom para evitar duplicados)
            if (await _context.Paises.AnyAsync(p => p.NomePais.ToLower() == pais.NomePais.ToLower()))
            {
                ModelState.AddModelError("NomePais", "Já existe um país com este nome.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(pais);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"País '{pais.NomePais}' criado com sucesso com ID: {pais.PaisId}.");
                    TempData["MensagemSucesso"] = $"País '{pais.NomePais}' adicionado com sucesso!";
                    // return RedirectToAction(nameof(Index)); // Se tiver uma lista de países
                    return RedirectToAction(nameof(Create)); // Redireciona para criar outro, com mensagem de sucesso
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, $"Erro ao criar país '{pais.NomePais}'.");
                    ModelState.AddModelError(string.Empty, "Não foi possível guardar o país. Verifique se já existe ou tente novamente. Se o problema persistir, contacte o suporte.");
                    TempData["MensagemErro"] = "Erro ao adicionar o país.";
                }
            }

            // Se ModelState não for válido ou ocorrer um erro, retorna à view com o objeto pais e os erros
            return View(pais);
        }

        // GET: Paises (Opcional: Uma view para listar os países)
        // public async Task<IActionResult> Index()
        // {
        //     return View(await _context.Paises.OrderBy(p => p.NomePais).ToListAsync());
        // }
    }
}
