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
            _logger.LogInformation("A aceder à action GET Create para Localidade.");
            try
            {
                // Popula o ViewBag.PaisesList com a lista de países para o dropdown
                // Ordenado por NomePais para melhor usabilidade
                var paises = await _context.Paises.OrderBy(p => p.NomePais).ToListAsync();
                ViewBag.PaisesList = new SelectList(
                    paises,
                    "PaisId",       // A propriedade do objeto Pai que será o 'value' da option
                    "NomePais"      // A propriedade do objeto Pai que será o texto visível da option
                );
                _logger.LogInformation($"ViewBag.PaisesList populado com {paises.Count} países.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao popular ViewBag.PaisesList na action GET Create para Localidade.");
                // Mesmo que falhe, crie uma lista vazia para evitar erro na view,
                // e adicione uma mensagem para o utilizador ou log.
                ViewBag.PaisesList = new List<SelectListItem>();
                ModelState.AddModelError(string.Empty, "Não foi possível carregar a lista de países. Tente novamente mais tarde.");
                // Você pode optar por retornar uma view de erro diferente aqui se a lista de países for crítica.
            }

            // Passa um novo objeto Localidade para o formulário.
            // Como a view agora se chama "Create.cshtml" e está em "Views/Locality/",
            // o ASP.NET Core MVC irá encontrá-la automaticamente por convenção.
            return View(new Localidade());
        }

        // POST: Localidades/Create
        // No seu LocalityController.cs

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NomeLocalidade,Regiao,PaisId")] Localidade localidade)
        {
            // PONTO 1: ModelState.IsValid
            // Se esta condição for falsa, ele vai para o final e tenta repopular o ViewBag.
            if (ModelState.IsValid)
            {
                try // Adicionado try-catch mais interno para a lógica principal
                {
                    // PONTO 2: Verificação de Duplicados (Se esta consulta falhar, pode ser um problema de BD/mapeamento)
                    bool localidadeJaExiste = await _context.Localidades.AnyAsync(l =>
                        l.NomeLocalidade.ToLower() == localidade.NomeLocalidade.ToLower() &&
                        (string.IsNullOrEmpty(l.Regiao) && string.IsNullOrEmpty(localidade.Regiao) || l.Regiao.ToLower() == localidade.Regiao.ToLower()) &&
                        l.PaisId == localidade.PaisId);

                    if (localidadeJaExiste)
                    {
                        ModelState.AddModelError(string.Empty, "Já existe uma localidade com este nome e região para o país selecionado.");
                        // Vai para o bloco final de repopular ViewBag e retornar View(localidade)
                    }
                    else
                    {
                        // PONTO 3: _context.Add(localidade);
                        // Aqui, o EF Core começa a rastrear a entidade 'localidade'.
                        // A propriedade 'localidade.PaisId' deve ter um valor válido vindo do formulário.
                        // O objeto 'localidade.Pais' (propriedade de navegação) provavelmente será NULO aqui,
                        // e isso é NORMAL para entidades recém-criadas onde você apenas define a FK.
                        _context.Add(localidade);

                        // PONTO 4: await _context.SaveChangesAsync();
                        // ESTE É O PONTO MAIS CRÍTICO ONDE OS ERROS DE BASE DE DADOS ACONTECEM.
                        // - Violação de chave estrangeira (PaisId não existe na tabela 'pais')
                        // - Campo obrigatório (NOT NULL na BD) está nulo no objeto 'localidade'
                        // - Problema de tipo de dados
                        // - Problemas de conexão com o TiDB Cloud (improvável se o GET funciona)
                        await _context.SaveChangesAsync();

                        _logger.LogInformation($"Localidade '{localidade.NomeLocalidade}' (ID: {localidade.LocalidadeId}) criada com sucesso para o País ID: {localidade.PaisId}.");
                        TempData["MensagemSucesso"] = $"Localidade '{localidade.NomeLocalidade}' adicionada com sucesso!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, $"Erro de DbUpdateException ao criar localidade '{localidade?.NomeLocalidade}'. InnerException: {dbEx.InnerException?.Message}");
                    ModelState.AddModelError(string.Empty, "Não foi possível guardar a localidade devido a um erro na base de dados. Verifique os dados inseridos.");
                    TempData["MensagemErro"] = "Erro ao adicionar localidade (BD).";
                    // Vai para o bloco final de repopular ViewBag e retornar View(localidade)
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Erro inesperado ao criar localidade '{localidade?.NomeLocalidade}'.");
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro inesperado. Tente novamente.");
                    TempData["MensagemErro"] = "Erro inesperado ao adicionar localidade.";
                    // Vai para o bloco final de repopular ViewBag e retornar View(localidade)
                }
            }
            else
            {
                _logger.LogWarning("ModelState inválido ao tentar criar Localidade. Erros: {ModelStateErrors}",
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                // ModelState já é inválido, vai para o bloco final de repopular ViewBag e retornar View(localidade)
            }

            // Bloco de fallback: Se ModelState não for válido OU se ocorreu uma exceção apanhada acima.
            _logger.LogInformation("Retornando à view Create de Localidade devido a erro ou falha na validação para Localidade: {NomeLocalidade}", localidade?.NomeLocalidade);
            try
            {
                ViewBag.PaisesList = new SelectList(
                    await _context.Paises.OrderBy(p => p.NomePais).ToListAsync(),
                    "PaisId",
                    "NomePais",
                    localidade?.PaisId // Tenta pré-selecionar o país
                );
            }
            catch (Exception selectListEx)
            {
                _logger.LogError(selectListEx, "Falha crítica ao tentar repopular PaisesList no catch da action Create POST de Localidade.");
                ViewBag.PaisesList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Erro ao carregar países --" } };
                ModelState.AddModelError(string.Empty, "Não foi possível carregar a lista de países para o formulário. Tente novamente.");
            }

            return View("CreateLocality", localidade);
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
