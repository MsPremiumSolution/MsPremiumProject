using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Models;
using System.Linq; // Adicionar para OrderBy e ToListAsync
using System.Threading.Tasks; // Adicionar para Task

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

        // GET: Locality
        public async Task<IActionResult> Index()
        {
            var localidades = await _context.Localidades
                                          .Include(l => l.Pais) // Inclui a entidade Pai para aceder a NomePais
                                          .OrderBy(l => l.Pais.NomePais)
                                          .ThenBy(l => l.NomeLocalidade)
                                          .ToListAsync();
            return View(localidades);
        }

        // Função auxiliar para popular o ViewBag com a lista de países
        private async Task PopulatePaisesDropDownList(object? selectedPais = null)
        {
            try
            {
                var paisesQuery = _context.Paises.OrderBy(p => p.NomePais);
                ViewBag.PaisesList = new SelectList(await paisesQuery.ToListAsync(), "PaisId", "NomePais", selectedPais);
                _logger.LogInformation("ViewBag.PaisesList populado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao popular ViewBag.PaisesList.");
                ViewBag.PaisesList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Erro ao carregar países --" } };
                // Considerar adicionar um erro ao ModelState se for crítico para a view
                // ModelState.AddModelError(string.Empty, "Não foi possível carregar a lista de países. Tente novamente mais tarde.");
            }
        }

        // GET: Locality/Create
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Acedendo à action GET Create para Localidade.");
            await PopulatePaisesDropDownList();
            // Assume que a tua view se chama "CreateLocality.cshtml"
            // Se se chamasse "Create.cshtml", poderias usar apenas return View(new Localidade());
            return View("CreateLocality", new Localidade());
        }

        // POST: Locality/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NomeLocalidade,Regiao,PaisId")] Localidade localidade)
        {
            _logger.LogInformation("Tentativa de POST Create para Localidade. Nome: {NomeLocalidade}, Regiao: {Regiao}, PaisId: {PaisId}",
                localidade.NomeLocalidade, localidade.Regiao, localidade.PaisId);

            // Normalizar Regiao para consistência (null se vazia ou apenas espaços)
            if (string.IsNullOrWhiteSpace(localidade.Regiao))
            {
                localidade.Regiao = null;
            }

            // Validação explícita do PaisId para garantir que não é 0 (se o [Range] não for suficiente ou para feedback melhor)
            if (localidade.PaisId == 0)
            {
                ModelState.AddModelError("PaisId", "É obrigatório selecionar um país.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Normalizar strings para verificação de duplicados (case-insensitive)
                    string nomeLocalidadeLower = localidade.NomeLocalidade.Trim().ToLower();
                    // Se Regiao for null, tratar como uma string vazia para a consulta de duplicados pode ser uma opção,
                    // ou comparar especificamente com null. A abordagem aqui trata null como distinto de string vazia
                    // para a lógica de negócio, mas para comparação de duplicados, podemos normalizar.
                    // Para simplificar: se Regiao é null, então l.Regiao deve ser null. Se Regiao tem valor, compara os valores.
                    // Se Regiao for opcional e pode ser "" ou null, e ambos significam "sem região", normalizar para null.

                    bool localidadeJaExiste = await _context.Localidades.AnyAsync(l =>
                        l.NomeLocalidade.ToLower() == nomeLocalidadeLower &&
                        (string.IsNullOrEmpty(l.Regiao) && localidade.Regiao == null || (l.Regiao != null && localidade.Regiao != null && l.Regiao.ToLower() == localidade.Regiao.ToLower())) &&
                        l.PaisId == localidade.PaisId);

                    if (localidadeJaExiste)
                    {
                        ModelState.AddModelError(string.Empty, "Já existe uma localidade com este nome e região para o país selecionado.");
                        _logger.LogWarning("Tentativa de criar localidade duplicada: {Nome}, {Regiao}, {PaisId}", localidade.NomeLocalidade, localidade.Regiao, localidade.PaisId);
                    }
                    else
                    {
                        _context.Add(localidade);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Localidade '{NomeLocalidade}' (ID: {LocalidadeId}) criada com sucesso para o País ID: {PaisId}.",
                                               localidade.NomeLocalidade, localidade.LocalidadeId, localidade.PaisId);
                        TempData["MensagemSucesso"] = $"Localidade '{localidade.NomeLocalidade}' adicionada com sucesso!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "Erro de DbUpdateException ao criar localidade '{NomeLocalidade}'. InnerException: {InnerException}",
                                     localidade.NomeLocalidade, dbEx.InnerException?.Message);
                    ModelState.AddModelError(string.Empty, "Não foi possível guardar a localidade. Verifique se o país selecionado é válido e se os dados estão corretos. Detalhe: " + (dbEx.InnerException?.Message ?? dbEx.Message));
                    TempData["MensagemErro"] = "Erro ao adicionar localidade (Base de Dados).";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado ao criar localidade '{NomeLocalidade}'.", localidade.NomeLocalidade);
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro inesperado ao tentar guardar a localidade.");
                    TempData["MensagemErro"] = "Erro inesperado ao adicionar localidade.";
                }
            }
            else
            {
                _logger.LogWarning("ModelState inválido ao tentar criar Localidade. Erros: {ModelStateErrors}",
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                TempData["MensagemErro"] = "Dados inválidos. Por favor, corrija os erros abaixo.";
            }

            // Se chegamos aqui, algo falhou (ModelState inválido ou exceção apanhada)
            // Repopular o dropdown de países
            await PopulatePaisesDropDownList(localidade.PaisId);
            return View("CreateLocality", localidade); // Retorna para a view CreateLocality com os dados e erros
        }

        // NOTA: A action `InserirLocalidade()` que tinhas parece redundante se `GET Create` já faz o trabalho
        // de preparar o formulário. Se era para um propósito diferente, podes mantê-la.
        // Se `InserirLocalidade` era apenas para mostrar o formulário, é melhor usar a convenção `Create`.
    }
}