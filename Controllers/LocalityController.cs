using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Models;
using System.Linq;
using System.Text.Json; // Para serializar os erros do ModelState
using System.Threading.Tasks;

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
                                          .Include(l => l.Pais)
                                          .OrderBy(l => l.Pais.NomePais)
                                          .ThenBy(l => l.NomeLocalidade)
                                          .ToListAsync();
            return View(localidades);
        }

        private async Task PopulatePaisesDropDownList(object? selectedPais = null)
        {
            try
            {
                var paises = await _context.Paises.OrderBy(p => p.NomePais).ToListAsync();
                ViewBag.PaisesList = new SelectList(paises, "PaisId", "NomePais", selectedPais);
                _logger.LogInformation("ViewBag.PaisesList populado com {Count} países.", paises.Count);
                if (paises.Any() && _logger.IsEnabled(LogLevel.Debug)) // Log de exemplo do primeiro país em Debug
                {
                    _logger.LogDebug("Primeiro país na lista para dropdown: ID={PaisId}, Nome='{NomePais}'", paises.First().PaisId, paises.First().NomePais);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao popular ViewBag.PaisesList.");
                ViewBag.PaisesList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Erro ao carregar países --" } };
            }
        }

        // GET: Locality/Create
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Acedendo à action GET Create para Localidade.");
            await PopulatePaisesDropDownList();
            return View("CreateLocality", new Localidade());
        }

        // POST: Locality/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // ***** SUGESTÃO PRINCIPAL: REMOVER O [Bind] PARA TESTE *****
        // Original: public async Task<IActionResult> Create([Bind("NomeLocalidade,Regiao,PaisId")] Localidade localidade)
        public async Task<IActionResult> Create(Localidade localidade) // Removido o [Bind]
        {
            // Log detalhado dos valores recebidos IMEDIATAMENTE
            _logger.LogInformation(
                "POST Create Localidade - Valores recebidos: NomeLocalidade='{NomeLocalidade}', Regiao='{Regiao}', PaisId={PaisId}",
                localidade.NomeLocalidade,
                localidade.Regiao,
                localidade.PaisId); // VERIFIQUE ESTE VALOR NOS LOGS DO RENDER

            // Normalizar Regiao para consistência (null se vazia ou apenas espaços)
            if (string.IsNullOrWhiteSpace(localidade.Regiao))
            {
                localidade.Regiao = null;
            }

            // Removendo a validação explícita de PaisId == 0 por agora,
            // pois os atributos [Required] e [Range] no modelo já devem cobrir isso.
            // Se PaisId ainda chegar como 0 e o ModelState estiver válido (improvável), podemos reavaliar.
            // if (localidade.PaisId == 0)
            // {
            //     ModelState.AddModelError("PaisId", "É obrigatório selecionar um país.");
            // }

            // Se a propriedade de navegação PaisId está preenchida e a propriedade de navegação Pais está a causar problemas de validação
            // (geralmente porque não é carregada e o EF Core tenta validar uma instância null com atributos [Required]),
            // podemos remover o erro do ModelState para a propriedade de navegação.
            if (localidade.PaisId > 0 && ModelState.ContainsKey(nameof(Localidade.Pais)))
            {
                // Apenas remove se for um erro específico de 'The Pais field is required.'
                // e não um erro vindo de validações dentro da classe Pai (se houver).
                var paisErrors = ModelState[nameof(Localidade.Pais)].Errors;
                if (paisErrors.Any(e => e.ErrorMessage.Contains("field is required"))) // Adapte a string se necessário
                {
                    _logger.LogInformation("Removendo erro de ModelState para a propriedade de navegação 'Pais' pois PaisId está preenchido.");
                    ModelState.Remove(nameof(Localidade.Pais));
                }
            }


            if (ModelState.IsValid)
            {
                try
                {
                    string nomeLocalidadeLower = localidade.NomeLocalidade.Trim().ToLower();
                    // Lógica de verificação de duplicados (mantida)
                    bool localidadeJaExiste = await _context.Localidades.AnyAsync(l =>
                        l.NomeLocalidade.ToLower() == nomeLocalidadeLower &&
                        ((string.IsNullOrEmpty(l.Regiao) && localidade.Regiao == null) || (l.Regiao != null && localidade.Regiao != null && l.Regiao.ToLower() == localidade.Regiao.ToLower())) &&
                        l.PaisId == localidade.PaisId);

                    if (localidadeJaExiste)
                    {
                        ModelState.AddModelError(string.Empty, "Já existe uma localidade com este nome e região para o país selecionado.");
                        _logger.LogWarning("Tentativa de criar localidade duplicada: Nome='{Nome}', Regiao='{Regiao}', PaisId={PaisId}",
                                           localidade.NomeLocalidade, localidade.Regiao, localidade.PaisId);
                    }
                    else
                    {
                        // Se PaisId é válido, a propriedade de navegação localidade.Pais será null aqui,
                        // o EF Core vai ligar a relação usando apenas o PaisId.
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
                // Log detalhado dos erros do ModelState
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .Select(ms => new { Field = ms.Key, Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToList() });
                string errorsJson = JsonSerializer.Serialize(errors); // Usar System.Text.Json

                _logger.LogWarning(
                    "ModelState inválido ao tentar criar Localidade. Detalhes dos Erros: {ModelStateErrorsDetailed} | Valores recebidos na Localidade: Nome='{NomeLocalidade}', Regiao='{Regiao}', PaisId={PaisId}",
                    errorsJson,
                    localidade.NomeLocalidade,
                    localidade.Regiao,
                    localidade.PaisId);
                TempData["MensagemErro"] = "Dados inválidos. Por favor, corrija os erros abaixo.";
            }

            await PopulatePaisesDropDownList(localidade.PaisId);
            return View("CreateLocality", localidade);
        }
    }
}