// File: Controllers/ClientController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSPremiumProject.Data;
using MSPremiumProject.Models;
using MSPremiumProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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

        // GET: Client (Lista de Clientes)
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Acedendo a GET Client/Index para listar Clientes.");
            try
            {
                var clientes = await _context.Clientes
                                         .Include(c => c.LocalidadeNavigation) // Ajusta para c.Localidade se renomeaste
                                             .ThenInclude(l => l.Pais)
                                         .OrderBy(c => c.Nome)
                                         .ThenBy(c => c.Apelido)
                                         .ToListAsync();
                return View(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar clientes na action Client/Index.");
                TempData["MensagemErro"] = "Ocorreu um erro ao tentar carregar a lista de clientes.";
                return View(new List<Cliente>());
            }
        }

        // GET: Client/Details/5
        public async Task<IActionResult> Details(ulong? id)
        {
            if (id == null || id == 0)
            {
                _logger.LogWarning("GET Client/Details - ID nulo ou inválido.");
                return NotFound();
            }

            _logger.LogInformation("Acedendo a GET Client/Details para ID: {ClienteId}", id);
            try
            {
                var cliente = await _context.Clientes
                    .Include(c => c.LocalidadeNavigation) // Ajusta se necessário
                        .ThenInclude(l => l.Pais)
                    .FirstOrDefaultAsync(m => m.ClienteId == id);

                if (cliente == null)
                {
                    _logger.LogWarning("GET Client/Details - Cliente com ID: {ClienteId} não encontrado.", id);
                    TempData["MensagemErro"] = "Cliente não encontrado.";
                    return NotFound();
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar detalhes do cliente ID: {ClienteId}", id);
                TempData["MensagemErro"] = "Ocorreu um erro ao tentar carregar os detalhes do cliente.";
                return RedirectToAction(nameof(Index));
            }
        }


        // GET: Client/Create (Já o tens, com ViewModel)
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Acedendo a GET Client/Create");
            var viewModel = new ClienteCreateViewModel();
            try
            {
                var regioes = await _context.Localidades.Select(l => l.Regiao).Where(r => !string.IsNullOrEmpty(r)).Distinct().OrderBy(r => r).ToListAsync();
                viewModel.RegioesList = regioes.Select(r => new SelectListItem { Value = r, Text = r }).ToList();
                viewModel.RegioesList.Insert(0, new SelectListItem { Value = "", Text = "-- Selecione uma Região --" });
            }
            catch (Exception ex) { _logger.LogError(ex, "Erro ao popular RegioesList em GET Create."); /* ... tratamento ... */ }
            viewModel.LocalidadesList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Selecione uma Região Primeiro --" } };
            return View(viewModel);
        }

        // POST: Client/Create (Já o tens, com ViewModel)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteCreateViewModel viewModel)
        {
            // ... (O teu código POST Create existente, validando e guardando viewModel.Cliente) ...
            // ... (Lógica de repopular dropdowns em caso de erro) ...

            // Certifica-te que este método está completo e correto como discutimos antes
            _logger.LogInformation("POST Client/Create. Cliente: {Nome}, LocalidadeId: {LocalidadeId}, RegiaoSelecionada: {Regiao}",
               viewModel.Cliente.Nome, viewModel.Cliente.LocalidadeId, viewModel.SelectedRegiao);

            if (viewModel.Cliente.LocalidadeId == 0)
            {
                ModelState.AddModelError("Cliente.LocalidadeId", "É obrigatório selecionar uma localidade válida.");
            }

            if (viewModel.Cliente.LocalidadeId > 0 && ModelState.TryGetValue(nameof(viewModel.Cliente) + "." + nameof(Models.Cliente.LocalidadeNavigation), out var navegacaoLocalidadeEntry))
            {
                if (navegacaoLocalidadeEntry.Errors.Any(e => e.ErrorMessage.Contains("field is required")))
                {
                    _logger.LogInformation("Removendo erro de ModelState para a propriedade de navegação 'Cliente.LocalidadeNavigation' pois Cliente.LocalidadeId ({LocalidadeId}) está preenchido.", viewModel.Cliente.LocalidadeId);
                    ModelState.Remove(nameof(viewModel.Cliente) + "." + nameof(Models.Cliente.LocalidadeNavigation));
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Clientes.Add(viewModel.Cliente);
                    await _context.SaveChangesAsync();
                    TempData["MensagemSucesso"] = $"Cliente '{viewModel.Cliente.Nome}' adicionado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx)
                { /* ... log e ModelState.AddModelError ... */
                    _logger.LogError(dbEx, "DbUpdateException ao criar cliente '{Nome}'. InnerException: {InnerMsg}", viewModel.Cliente.Nome, dbEx.InnerException?.Message);
                    ModelState.AddModelError(string.Empty, "Não foi possível guardar o cliente devido a um erro na base de dados. Verifique os dados. Detalhe: " + (dbEx.InnerException?.Message ?? dbEx.Message));
                }
                catch (Exception ex)
                { /* ... log e ModelState.AddModelError ... */
                    _logger.LogError(ex, "Erro inesperado ao criar cliente '{Nome}'.", viewModel.Cliente.Nome);
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro inesperado ao tentar guardar o cliente.");
                }
            }
            else
            { /* ... log de erros do ModelState ... */
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .Select(ms => new { Field = ms.Key, Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToList() });
                _logger.LogWarning("ModelState inválido ao tentar criar Cliente. Erros: {ModelStateErrorsJson}", JsonSerializer.Serialize(errors));
                TempData["MensagemErro"] = "Dados inválidos. Por favor, corrija os erros sinalizados.";
            }

            // Repopular dropdowns
            try
            {
                var regioesDb = await _context.Localidades.Select(l => l.Regiao).Where(r => !string.IsNullOrEmpty(r)).Distinct().OrderBy(r => r).ToListAsync();
                viewModel.RegioesList = regioesDb.Select(r => new SelectListItem { Value = r, Text = r }).ToList();
                viewModel.RegioesList.Insert(0, new SelectListItem { Value = "", Text = "-- Selecione uma Região --" });

                if (!string.IsNullOrEmpty(viewModel.SelectedRegiao))
                {
                    var localidadesDb = await _context.Localidades
                                                .Where(l => l.Regiao == viewModel.SelectedRegiao)
                                                .OrderBy(l => l.NomeLocalidade)
                                                .Select(l => new SelectListItem { Value = l.LocalidadeId.ToString(), Text = l.NomeLocalidade })
                                                .ToListAsync();
                    var localidadesParaDropdown = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Selecione uma Localidade --" } };
                    localidadesParaDropdown.AddRange(localidadesDb);
                    viewModel.LocalidadesList = localidadesParaDropdown;
                    // Tenta pré-selecionar a localidade se LocalidadeId estiver no cliente do viewmodel
                    foreach (var item in viewModel.LocalidadesList.Where(x => x.Value == viewModel.Cliente.LocalidadeId.ToString())) { item.Selected = true; }
                }
                else
                {
                    viewModel.LocalidadesList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Selecione uma Região Primeiro --" } };
                }
            }
            catch (Exception ex) { _logger.LogError(ex, "Erro ao REpopular dropdowns em POST Create."); /* ... tratamento ... */ }

            return View(viewModel);
        }

        // Action para AJAX (Já a tens)
        [HttpGet]
        public async Task<JsonResult> GetLocalidadesPorRegiao(string regiao)
        {
            // ... (O teu código GetLocalidadesPorRegiao existente) ...
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
                if (localidades.Any()) { localidadesList.Add(new SelectListItem { Value = "", Text = "-- Selecione uma Localidade --" }); localidadesList.AddRange(localidades); }
                else { localidadesList.Add(new SelectListItem { Value = "", Text = "-- Nenhuma localidade para esta região --" }); }
            }
            catch (Exception ex) { /* ... log ... */ }
            return Json(localidadesList);
        }


        // GET: Client/Edit/5
        public async Task<IActionResult> Edit(ulong? id)
        {
            if (id == null || id == 0) return NotFound();
            _logger.LogInformation("Acedendo a GET Client/Edit para ID: {ClienteId}", id);

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                _logger.LogWarning("GET Client/Edit - Cliente com ID: {ClienteId} não encontrado.", id);
                TempData["MensagemErro"] = "Cliente não encontrado.";
                return NotFound();
            }

            // Para o Edit, vamos precisar do ClienteCreateViewModel para popular os dropdowns
            // e pré-selecionar os valores corretos.
            var viewModel = new ClienteCreateViewModel
            {
                Cliente = cliente
            };

            try
            {
                // Popular Regiões
                var regioes = await _context.Localidades.Select(l => l.Regiao).Where(r => !string.IsNullOrEmpty(r)).Distinct().OrderBy(r => r).ToListAsync();
                viewModel.RegioesList = regioes.Select(r => new SelectListItem { Value = r, Text = r }).ToList();
                viewModel.RegioesList.Insert(0, new SelectListItem { Value = "", Text = "-- Selecione uma Região --" });

                // Pré-selecionar a região do cliente (se a localidade estiver carregada)
                var localidadeDoCliente = await _context.Localidades.FindAsync(cliente.LocalidadeId);
                if (localidadeDoCliente != null)
                {
                    viewModel.SelectedRegiao = localidadeDoCliente.Regiao;
                    // Popular Localidades para a região pré-selecionada
                    var localidadesDaRegiao = await _context.Localidades
                                                       .Where(l => l.Regiao == localidadeDoCliente.Regiao)
                                                       .OrderBy(l => l.NomeLocalidade)
                                                       .Select(l => new SelectListItem { Value = l.LocalidadeId.ToString(), Text = l.NomeLocalidade })
                                                       .ToListAsync();
                    var localidadesParaDropdown = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Selecione uma Localidade --" } };
                    localidadesParaDropdown.AddRange(localidadesDaRegiao);
                    viewModel.LocalidadesList = localidadesParaDropdown;
                    // Pré-selecionar a localidade no dropdown
                    foreach (var item in viewModel.LocalidadesList.Where(x => x.Value == cliente.LocalidadeId.ToString())) { item.Selected = true; }
                }
                else
                {
                    viewModel.LocalidadesList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Selecione uma Região Primeiro --" } };
                }
            }
            catch (Exception ex) { _logger.LogError(ex, "Erro ao popular dropdowns em GET Client/Edit."); /* ... tratamento ... */ }

            return View(viewModel); // Usaremos a mesma view Create.cshtml para o Edit, ou uma view Edit.cshtml similar.
                                    // Se usares a mesma view Create.cshtml, ela precisa de ser adaptada para lidar com edição.
                                    // É mais comum ter uma Edit.cshtml separada.
                                    // Por simplicidade, vou assumir que tens uma View/Client/Edit.cshtml que aceita ClienteCreateViewModel
        }

        // POST: Client/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ulong id, ClienteCreateViewModel viewModel)
        {
            if (id != viewModel.Cliente.ClienteId)
            {
                _logger.LogWarning("POST Client/Edit - ID de rota ({RouteId}) não corresponde ao ID do modelo ({ModelId}).", id, viewModel.Cliente.ClienteId);
                return NotFound();
            }
            _logger.LogInformation("POST Client/Edit para Cliente ID: {ClienteId}. LocalidadeId: {LocalidadeId}, RegiaoSelecionada: {Regiao}",
               viewModel.Cliente.ClienteId, viewModel.Cliente.LocalidadeId, viewModel.SelectedRegiao);


            if (viewModel.Cliente.LocalidadeId == 0)
            {
                ModelState.AddModelError("Cliente.LocalidadeId", "É obrigatório selecionar uma localidade válida.");
            }

            if (viewModel.Cliente.LocalidadeId > 0 && ModelState.TryGetValue(nameof(viewModel.Cliente) + "." + nameof(Models.Cliente.LocalidadeNavigation), out var navegacaoLocalidadeEntry))
            {
                if (navegacaoLocalidadeEntry.Errors.Any(e => e.ErrorMessage.Contains("field is required")))
                {
                    ModelState.Remove(nameof(viewModel.Cliente) + "." + nameof(Models.Cliente.LocalidadeNavigation));
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewModel.Cliente);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Cliente '{Nome}' (ID: {ClienteId}) atualizado com sucesso.", viewModel.Cliente.Nome, viewModel.Cliente.ClienteId);
                    TempData["MensagemSucesso"] = $"Cliente '{viewModel.Cliente.Nome}' atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException dbUcEx)
                {
                    _logger.LogError(dbUcEx, "DbUpdateConcurrencyException ao editar cliente ID: {ClienteId}.", viewModel.Cliente.ClienteId);
                    if (!await ClienteExists(viewModel.Cliente.ClienteId)) { return NotFound(); } else { throw; }
                }
                catch (DbUpdateException dbEx)
                { /* ... log e ModelState.AddModelError ... */
                    _logger.LogError(dbEx, "DbUpdateException ao editar cliente '{Nome}'. InnerException: {InnerMsg}", viewModel.Cliente.Nome, dbEx.InnerException?.Message);
                    ModelState.AddModelError(string.Empty, "Não foi possível atualizar o cliente. Verifique os dados. Detalhe: " + (dbEx.InnerException?.Message ?? dbEx.Message));
                }
                catch (Exception ex)
                { /* ... log e ModelState.AddModelError ... */
                    _logger.LogError(ex, "Erro inesperado ao editar cliente '{Nome}'.", viewModel.Cliente.Nome);
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro inesperado ao tentar atualizar o cliente.");
                }
            }
            else
            {
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .Select(ms => new { Field = ms.Key, Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToList() });
                _logger.LogWarning("ModelState inválido ao tentar editar Cliente ID: {ClienteId}. Erros: {ModelStateErrorsJson}", viewModel.Cliente.ClienteId, JsonSerializer.Serialize(errors));
                TempData["MensagemErro"] = "Dados inválidos. Por favor, corrija os erros sinalizados.";
            }

            // Repopular dropdowns em caso de erro
            try
            {
                var regioesDb = await _context.Localidades.Select(l => l.Regiao).Where(r => !string.IsNullOrEmpty(r)).Distinct().OrderBy(r => r).ToListAsync();
                viewModel.RegioesList = regioesDb.Select(r => new SelectListItem { Value = r, Text = r }).ToList();
                viewModel.RegioesList.Insert(0, new SelectListItem { Value = "", Text = "-- Selecione uma Região --" });

                if (!string.IsNullOrEmpty(viewModel.SelectedRegiao))
                {
                    var localidadesDb = await _context.Localidades
                                                .Where(l => l.Regiao == viewModel.SelectedRegiao)
                                                .OrderBy(l => l.NomeLocalidade)
                                                .Select(l => new SelectListItem { Value = l.LocalidadeId.ToString(), Text = l.NomeLocalidade })
                                                .ToListAsync();
                    var localidadesParaDropdown = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Selecione uma Localidade --" } };
                    localidadesParaDropdown.AddRange(localidadesDb);
                    viewModel.LocalidadesList = localidadesParaDropdown;
                    foreach (var item in viewModel.LocalidadesList.Where(x => x.Value == viewModel.Cliente.LocalidadeId.ToString())) { item.Selected = true; }
                }
                else
                {
                    viewModel.LocalidadesList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Selecione uma Região Primeiro --" } };
                }
            }
            catch (Exception ex) { _logger.LogError(ex, "Erro ao REpopular dropdowns em POST Edit."); /* ... tratamento ... */ }

            return View(viewModel); // Retorna para a view Edit (que deve ser similar à Create)
        }

        // GET: Client/Delete/5
        public async Task<IActionResult> Delete(ulong? id)
        {
            if (id == null || id == 0) return NotFound();
            _logger.LogInformation("Acedendo a GET Client/Delete para ID: {ClienteId}", id);

            try
            {
                var cliente = await _context.Clientes
                    .Include(c => c.LocalidadeNavigation) // Ajusta se necessário
                        .ThenInclude(l => l.Pais)
                    .FirstOrDefaultAsync(m => m.ClienteId == id);

                if (cliente == null)
                {
                    _logger.LogWarning("GET Client/Delete - Cliente com ID: {ClienteId} não encontrado.", id);
                    TempData["MensagemErro"] = "Cliente não encontrado.";
                    return NotFound();
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cliente para apagar, ID: {ClienteId}", id);
                TempData["MensagemErro"] = "Ocorreu um erro ao tentar carregar dados para apagar o cliente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ulong id)
        {
            _logger.LogInformation("Acedendo a POST Client/DeleteConfirmed para ID: {ClienteId}", id);
            if (id == 0) return NotFound(); // Ou BadRequest

            try
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente != null)
                {
                    _context.Clientes.Remove(cliente);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Cliente ID: {ClienteId} apagado com sucesso.", id);
                    TempData["MensagemSucesso"] = "Cliente apagado com sucesso.";
                }
                else
                {
                    _logger.LogWarning("POST Client/DeleteConfirmed - Cliente com ID: {ClienteId} não encontrado para apagar.", id);
                    TempData["MensagemErro"] = "Cliente não encontrado para apagar.";
                }
            }
            catch (DbUpdateException dbEx) // Captura erros específicos de FK se ON DELETE RESTRICT estiver ativo
            {
                _logger.LogError(dbEx, "DbUpdateException ao apagar cliente ID: {ClienteId}. InnerException: {InnerMsg}", id, dbEx.InnerException?.Message);
                TempData["MensagemErro"] = "Não foi possível apagar o cliente. Pode estar associado a outros registos (ex: Propostas). Detalhe: " + (dbEx.InnerException?.Message ?? dbEx.Message);
                return RedirectToAction(nameof(Delete), new { id = id }); // Volta para a página de confirmação com erro
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao apagar cliente ID: {ClienteId}.", id);
                TempData["MensagemErro"] = "Ocorreu um erro inesperado ao tentar apagar o cliente.";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ClienteExists(ulong id)
        {
            return await _context.Clientes.AnyAsync(e => e.ClienteId == id);
        }
    }
}