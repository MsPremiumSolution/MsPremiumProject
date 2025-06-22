// File: Controllers/ClienteController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Models;
using MSPremiumProject.ViewModels; // Adicionar o using para o ViewModel
using System.Linq;
using System.Text.Json; // Para serializar erros do ModelState
using System.Threading.Tasks;
using System.Collections.Generic; // Para List

namespace MSPremiumProject.Controllers
{
    public class ClienteController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClienteController> _logger;

        public ClienteController(AppDbContext context, ILogger<ClienteController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Cliente/Create
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Acedendo a GET Cliente/Create");
            var viewModel = new ClienteCreateViewModel();

            try
            {
                var regioes = await _context.Localidades
                                        .Select(l => l.Regiao)
                                        .Where(r => !string.IsNullOrEmpty(r))
                                        .Distinct()
                                        .OrderBy(r => r)
                                        .ToListAsync();

                viewModel.RegioesList = regioes.Select(r => new SelectListItem { Value = r, Text = r }).ToList();
                viewModel.RegioesList.Insert(0, new SelectListItem { Value = "", Text = "-- Selecione uma Região --" });
                _logger.LogInformation("Lista de Regiões populada com {Count} regiões.", regioes.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao popular lista de Regiões em GET Cliente/Create.");
                viewModel.RegioesList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Erro ao carregar Regiões --" } };
                // Opcional: Adicionar um erro ao ModelState para a view
                // ModelState.AddModelError(string.Empty, "Não foi possível carregar as regiões.");
            }

            // Inicializar a lista de localidades vazia ou com placeholder
            viewModel.LocalidadesList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Selecione uma Região Primeiro --" } };

            return View(viewModel);
        }

        // Action para AJAX: Buscar localidades por região
        [HttpGet]
        public async Task<JsonResult> GetLocalidadesPorRegiao(string regiao)
        {
            _logger.LogInformation("GetLocalidadesPorRegiao chamada para Regiao: {Regiao}", regiao);
            var localidadesList = new List<SelectListItem>();

            if (string.IsNullOrEmpty(regiao))
            {
                localidadesList.Add(new SelectListItem { Value = "", Text = "-- Selecione uma Região Primeiro --" });
                return Json(localidadesList);
            }

            try
            {
                var localidades = await _context.Localidades
                                            .Where(l => l.Regiao == regiao)
                                            .OrderBy(l => l.NomeLocalidade)
                                            .Select(l => new SelectListItem
                                            {
                                                Value = l.LocalidadeId.ToString(),
                                                Text = l.NomeLocalidade
                                            })
                                            .ToListAsync();

                if (localidades.Any())
                {
                    localidadesList.Add(new SelectListItem { Value = "", Text = "-- Selecione uma Localidade --" });
                    localidadesList.AddRange(localidades);
                }
                else
                {
                    localidadesList.Add(new SelectListItem { Value = "", Text = "-- Nenhuma localidade para esta região --" });
                }
                _logger.LogInformation("Retornando {Count} localidades para a região '{Regiao}'.", localidades.Count, regiao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro em GetLocalidadesPorRegiao para Regiao: {Regiao}", regiao);
                localidadesList.Clear(); // Limpa em caso de erro
                localidadesList.Add(new SelectListItem { Value = "", Text = "-- Erro ao carregar Localidades --" });
            }
            return Json(localidadesList);
        }

        // POST: Cliente/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteCreateViewModel viewModel)
        {
            _logger.LogInformation("POST Cliente/Create. Cliente: {Nome}, LocalidadeId: {LocalidadeId}, RegiaoSelecionada: {Regiao}",
                viewModel.Cliente.Nome, viewModel.Cliente.LocalidadeId, viewModel.SelectedRegiao);

            // Se LocalidadeId é 0 (ou não selecionado), mas [Required] está no modelo, ModelState já deve ser inválido.
            // A validação de LocalidadeId > 0 é uma dupla verificação.
            if (viewModel.Cliente.LocalidadeId == 0)
            {
                ModelState.AddModelError("Cliente.LocalidadeId", "É obrigatório selecionar uma localidade válida.");
            }

            // O EF Core pode tentar validar a propriedade de navegação 'Localidade'
            // mesmo que estejamos apenas a definir 'LocalidadeId'. Se 'LocalidadeId' for válida,
            // e 'Localidade' (instância) estiver a causar um erro de validação (ex: "The Localidade field is required.")
            // podemos remover esse erro específico.
            if (viewModel.Cliente.LocalidadeId > 0 && ModelState.TryGetValue(nameof(viewModel.Cliente) + "." + nameof(Cliente.LocalidadeId), out var localidadeEntry))
            {
                // Verifica se o erro é o específico de required para a instância, não para LocalidadeId
                if (localidadeEntry.Errors.Any(e => e.ErrorMessage.Contains("field is required")))
                {
                    _logger.LogInformation("Removendo erro de ModelState para a propriedade de navegação 'Cliente.Localidade' pois Cliente.LocalidadeId ({LocalidadeId}) está preenchido.", viewModel.Cliente.LocalidadeId);
                    ModelState.Remove(nameof(viewModel.Cliente) + "." + nameof(Cliente.LocalidadeId));
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Clientes.Add(viewModel.Cliente); // Adiciona o Cliente do ViewModel
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Cliente '{Nome}' (ID: {ClienteId}) criado com sucesso.", viewModel.Cliente.Nome, viewModel.Cliente.ClienteId);
                    TempData["MensagemSucesso"] = $"Cliente '{viewModel.Cliente.Nome}' adicionado com sucesso!";
                    return RedirectToAction(nameof(Create)); // Redireciona para um novo formulário de criação ou para o Index de Clientes
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "DbUpdateException ao criar cliente '{Nome}'. InnerException: {InnerMsg}", viewModel.Cliente.Nome, dbEx.InnerException?.Message);
                    ModelState.AddModelError(string.Empty, "Não foi possível guardar o cliente devido a um erro na base de dados. Verifique os dados. Detalhe: " + (dbEx.InnerException?.Message ?? dbEx.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado ao criar cliente '{Nome}'.", viewModel.Cliente.Nome);
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro inesperado ao tentar guardar o cliente.");
                }
            }
            else
            {
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .Select(ms => new { Field = ms.Key, Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToList() });
                _logger.LogWarning("ModelState inválido ao tentar criar Cliente. Erros: {ModelStateErrorsJson}", JsonSerializer.Serialize(errors));
                TempData["MensagemErro"] = "Dados inválidos. Por favor, corrija os erros sinalizados.";
            }

            // Se chegamos aqui, algo falhou (ModelState inválido ou exceção). Repopular dropdowns.
            // Repopular RegioesList
            try
            {
                var regioesDb = await _context.Localidades.Select(l => l.Regiao).Where(r => !string.IsNullOrEmpty(r)).Distinct().OrderBy(r => r).ToListAsync();
                viewModel.RegioesList = regioesDb.Select(r => new SelectListItem { Value = r, Text = r }).ToList();
                viewModel.RegioesList.Insert(0, new SelectListItem { Value = "", Text = "-- Selecione uma Região --" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao REpopular lista de Regiões em POST Cliente/Create.");
                viewModel.RegioesList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Erro ao carregar Regiões --" } };
            }


            // Repopular LocalidadesList com base na SelectedRegiao que o utilizador tinha submetido (se houver)
            if (!string.IsNullOrEmpty(viewModel.SelectedRegiao))
            {
                try
                {
                    var localidadesDb = await _context.Localidades
                                                .Where(l => l.Regiao == viewModel.SelectedRegiao)
                                                .OrderBy(l => l.NomeLocalidade)
                                                .Select(l => new SelectListItem { Value = l.LocalidadeId.ToString(), Text = l.NomeLocalidade })
                                                .ToListAsync();
                    var localidadesParaDropdown = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Selecione uma Localidade --" } };
                    localidadesParaDropdown.AddRange(localidadesDb);
                    viewModel.LocalidadesList = localidadesParaDropdown;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao REpopular lista de Localidades em POST Cliente/Create para Regiao: {Regiao}", viewModel.SelectedRegiao);
                    viewModel.LocalidadesList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Erro ao carregar Localidades --" } };
                }
            }
            else
            {
                viewModel.LocalidadesList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Selecione uma Região Primeiro --" } };
            }

            return View(viewModel);
        }
    }
}