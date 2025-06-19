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

        public async Task<IActionResult> Index()
        {
            // Busca todas as localidades, incluindo a informação do País associado,
            // e ordena por nome do país e depois por nome da localidade.
            var localidades = await _context.Localidades
                                          .Include(l => l.Pais) // Inclui a entidade Pai para aceder a NomePais
                                          .OrderBy(l => l.Pais.NomePais)
                                          .ThenBy(l => l.NomeLocalidade)
                                          .ToListAsync();
            return View(localidades);
        }

        // GET: Localidades/Create
        public async Task<IActionResult> Create()
        {
            // Popula o ViewBag.PaisesList com a lista de países para o dropdown
            // Ordenado por NomePais para melhor usabilidade
            ViewBag.PaisesList = new SelectList(await _context.Paises.OrderBy(p => p.NomePais).ToListAsync(), "PaisId", "NomePais");
            return View(new Localidade()); // Passa um novo objeto Localidade para o formulário
        }

        // POST: Localidades/Create
        // No seu LocalityController.cs

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NomeLocalidade,Regiao,PaisId")] Localidade localidade)
        {
            // Tenta executar a lógica de criação
            try
            {
                // 1. Verifica se o modelo recebido do formulário é válido com base nas DataAnnotations
                if (ModelState.IsValid)
                {
                    // 2. (Opcional, mas recomendado) Verifica se já existe uma localidade similar para evitar duplicados
                    bool localidadeJaExiste = await _context.Localidades.AnyAsync(l =>
                        l.NomeLocalidade.ToLower() == localidade.NomeLocalidade.ToLower() &&
                        (string.IsNullOrEmpty(l.Regiao) && string.IsNullOrEmpty(localidade.Regiao) || l.Regiao.ToLower() == localidade.Regiao.ToLower()) && // Compara Regiao, tratando nulos
                        l.PaisId == localidade.PaisId);

                    if (localidadeJaExiste)
                    {
                        ModelState.AddModelError(string.Empty, "Já existe uma localidade com este nome e região para o país selecionado.");
                        _logger.LogWarning($"Tentativa de criar localidade duplicada: Nome='{localidade.NomeLocalidade}', Regiao='{localidade.Regiao}', PaisID='{localidade.PaisId}'.");
                        // Se duplicado, precisa repopular o ViewBag e retornar à view
                    }
                    else
                    {
                        // 3. Se o modelo é válido e a localidade não é duplicada, adiciona ao DbContext
                        _context.Add(localidade);
                        // 4. Tenta salvar as alterações na base de dados
                        await _context.SaveChangesAsync();

                        _logger.LogInformation($"Localidade '{localidade.NomeLocalidade}' (ID: {localidade.LocalidadeId}) criada com sucesso para o País ID: {localidade.PaisId}.");
                        TempData["MensagemSucesso"] = $"Localidade '{localidade.NomeLocalidade}' adicionada com sucesso!";
                        return RedirectToAction(nameof(Index)); // Redireciona para a lista de localidades após sucesso
                    }
                }
                else
                {
                    // Se ModelState não é válido inicialmente (ex: campo obrigatório em falta)
                    _logger.LogWarning("ModelState inválido ao tentar criar Localidade. Erros: {ModelStateErrors}",
                        string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                }
            }
            catch (DbUpdateException dbEx) // Captura erros específicos do Entity Framework ao interagir com a BD
            {
                _logger.LogError(dbEx, $"Erro de DbUpdateException ao criar localidade '{localidade?.NomeLocalidade}'. InnerException: {dbEx.InnerException?.Message}");
                ModelState.AddModelError(string.Empty, "Não foi possível guardar a localidade devido a um erro na base de dados. Verifique os dados inseridos. Se o problema persistir, contacte o suporte.");
                TempData["MensagemErro"] = "Erro ao adicionar localidade (problema na base de dados).";
            }
            catch (Exception ex) // Captura quaisquer outros erros inesperados durante o processo
            {
                _logger.LogError(ex, $"Erro inesperado ao criar localidade '{localidade?.NomeLocalidade}'.");
                ModelState.AddModelError(string.Empty, "Ocorreu um erro inesperado ao processar o seu pedido. Tente novamente.");
                TempData["MensagemErro"] = "Erro inesperado ao adicionar localidade.";
            }

            // Se chegou aqui, significa que:
            // - ModelState não era válido inicialmente, OU
            // - Uma localidade duplicada foi detetada, OU
            // - Ocorreu uma exceção ao tentar salvar na base de dados.
            // Em todos estes casos, precisamos de retornar à view 'Create' com o objeto 'localidade'
            // e os erros no ModelState, e crucialmente, repopular o ViewBag.PaisesList.

            _logger.LogInformation("Retornando à view Create de Localidade devido a erro ou falha na validação.");
            try
            {
                ViewBag.PaisesList = new SelectList(
                    await _context.Paises.OrderBy(p => p.NomePais).ToListAsync(),
                    "PaisId",
                    "NomePais",
                    localidade?.PaisId // Pré-seleciona o país que o utilizador tinha escolhido, se 'localidade' não for nula
                );
            }
            catch (Exception selectListEx)
            {
                _logger.LogError(selectListEx, "Falha crítica ao tentar repopular PaisesList no catch da action Create POST de Localidade.");
                // Adiciona uma lista vazia para evitar erro na view, mas o utilizador não poderá selecionar um país
                ViewBag.PaisesList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Erro ao carregar países --" } };
                ModelState.AddModelError(string.Empty, "Não foi possível carregar a lista de países. Contacte o suporte.");
            }

            return View(localidade); // Retorna para a view Create, mostrando os erros de validação ou da exceção
        }

        public async Task<IActionResult> InserirLocalidade() // Mude para async Task<IActionResult>
        {
            // Popula o ViewBag.PaisesList com a lista de países para o dropdown
            ViewBag.PaisesList = new SelectList(
                await _context.Paises.OrderBy(p => p.NomePais).ToListAsync(),
                "PaisId",
                "NomePais"
            );
            return View("~/Views/Locality/CreateLocality.cshtml", new Localidade()); // Passa um novo objeto
        }
    }
}
