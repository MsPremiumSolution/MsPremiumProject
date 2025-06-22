// File: Controllers/ClientController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Adicionado para ILogger, se não estiver já num using global
using MSPremiumProject.Data;
using MSPremiumProject.Models;       // Adicionado para Cliente, Localidade, etc.
using MSPremiumProject.ViewModels;
using System;                       // Adicionado para Exception
using System.Collections.Generic;   // Adicionado para List
using System.Linq;
using System.Text.Json;             // Adicionado para JsonSerializer
using System.Threading.Tasks;

namespace MSPremiumProject.Controllers
{
    public class ClientController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClientController> _logger;

        public ClientController(AppDbContext context, ILogger<ClientController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Client/Create
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Acedendo a GET Client/Create");
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
                _logger.LogError(ex, "Erro ao popular lista de Regiões em GET Client/Create.");
                viewModel.RegioesList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Erro ao carregar Regiões --" } };
            }

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
                localidadesList.Clear();
                localidadesList.Add(new SelectListItem { Value = "", Text = "-- Erro ao carregar Localidades --" });
            }
            return Json(localidadesList);
        }

        // POST: Client/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteCreateViewModel viewModel)
        {
            _logger.LogInformation("POST Client/Create. Cliente: {Nome}, LocalidadeId: {LocalidadeId}, RegiaoSelecionada: {Regiao}",
                viewModel.Cliente.Nome, viewModel.Cliente.LocalidadeId, viewModel.SelectedRegiao);

            if (viewModel.Cliente.LocalidadeId == 0)
            {
                ModelState.AddModelError("Cliente.LocalidadeId", "É obrigatório selecionar uma localidade válida.");
            }

            // CORRIGIDO AQUI para usar nameof(Models.Cliente.LocalidadeNavigation)
            // O objetivo é remover o erro de validação da propriedade de NAVEGAÇÃO 'LocalidadeNavigation'
            // se a CHAVE ESTRANGEIRA 'LocalidadeId' estiver preenchida.
            if (viewModel.Cliente.LocalidadeId > 0 && ModelState.TryGetValue(nameof(viewModel.Cliente) + "." + nameof(Models.Cliente.LocalidadeNavigation), out var navegacaoLocalidadeEntry))
            {
                // Verifica se o erro é o específico de "field is required" para a instância da propriedade de navegação.
                if (navegacaoLocalidadeEntry.Errors.Any(e => e.ErrorMessage.Contains("field is required"))) // Adapta a string da mensagem de erro se necessário
                {
                    _logger.LogInformation("Removendo erro de ModelState para a propriedade de navegação 'Cliente.LocalidadeNavigation' pois Cliente.LocalidadeId ({LocalidadeId}) está preenchido.", viewModel.Cliente.LocalidadeId);
                    ModelState.Remove(nameof(viewModel.Cliente) + "." + nameof(Models.Cliente.LocalidadeNavigation));
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // A propriedade viewModel.Cliente já deve ter LocalidadeId preenchido.
                    // Não precisamos de definir viewModel.Cliente.LocalidadeNavigation manualmente aqui se não formos usá-la imediatamente.
                    // O EF Core vai ligar a relação usando apenas o LocalidadeId ao salvar.
                    _context.Clientes.Add(viewModel.Cliente);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Cliente '{Nome}' (ID: {ClienteId}) criado com sucesso.", viewModel.Cliente.Nome, viewModel.Cliente.ClienteId);
                    TempData["MensagemSucesso"] = $"Cliente '{viewModel.Cliente.Nome}' adicionado com sucesso!";
                    return RedirectToAction(nameof(Create)); // Ou para o Index de Clientes, ou para onde fizer mais sentido
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

            // Se chegamos aqui, algo falhou. Repopular dropdowns.
            try
            {
                var regioesDb = await _context.Localidades.Select(l => l.Regiao).Where(r => !string.IsNullOrEmpty(r)).Distinct().OrderBy(r => r).ToListAsync();
                viewModel.RegioesList = regioesDb.Select(r => new SelectListItem { Value = r, Text = r }).ToList();
                viewModel.RegioesList.Insert(0, new SelectListItem { Value = "", Text = "-- Selecione uma Região --" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao REpopular lista de Regiões em POST Client/Create.");
                viewModel.RegioesList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Erro ao carregar Regiões --" } };
            }

            if (!string.IsNullOrEmpty(viewModel.SelectedRegiao))
            {
                try
                {
                    var localidadesDb = await _context.Localidades
                                                .Where(l => l.Regiao == viewModel.SelectedRegiao)
                                                .OrderBy(l => l.NomeLocalidade)
                                                .Select(l => new SelectListItem { Value = l.LocalidadeId.ToString(), Text = l.NomeLocalidade })
                                                .ToListAsync();
                    // Preserva o valor selecionado de LocalidadeId se possível
                    var selectedLocalidadeId = viewModel.Cliente.LocalidadeId.ToString();

                    var localidadesParaDropdown = new List<SelectListItem>();
                    localidadesParaDropdown.Add(new SelectListItem { Value = "", Text = "-- Selecione uma Localidade --" });

                    foreach (var loc in localidadesDb)
                    {
                        localidadesParaDropdown.Add(new SelectListItem
                        {
                            Value = loc.Value,
                            Text = loc.Text,
                            Selected = loc.Value == selectedLocalidadeId
                        });
                    }
                    viewModel.LocalidadesList = localidadesParaDropdown;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao REpopular lista de Localidades em POST Client/Create para Regiao: {Regiao}", viewModel.SelectedRegiao);
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