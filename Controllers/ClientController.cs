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
                                         .Include(c => c.LocalidadeNavigation)
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
                    .Include(c => c.LocalidadeNavigation)
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

        // GET: Client/Create
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Acedendo a GET Client/Create (nova versão com países).");
            var viewModel = new ClienteCreateViewModel();
            try
            {
                viewModel.PaisesList = await _context.Paises
                    .OrderBy(p => p.NomePais)
                    .Select(p => new SelectListItem
                    {
                        Value = p.PaisId.ToString(),
                        Text = p.NomePais
                    })
                    .ToListAsync();
                var portugal = viewModel.PaisesList.FirstOrDefault(p => p.Text.Equals("Portugal", StringComparison.OrdinalIgnoreCase));
                if (portugal != null)
                {
                    viewModel.SelectedPaisId = ulong.Parse(portugal.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao popular PaisesList em GET Create.");
                TempData["MensagemErro"] = "Não foi possível carregar a lista de países.";
            }
            return View(viewModel);
        }

        // POST: Client/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteCreateViewModel viewModel)
        {
            _logger.LogInformation("POST Client/Create. País ID: {PaisId}, Localidade: {Localidade}", viewModel.SelectedPaisId, viewModel.NomeLocalidade);

            ModelState.Remove(nameof(viewModel.Cliente) + "." + nameof(Models.Cliente.LocalidadeNavigation));

            if (ModelState.IsValid)
            {
                try
                {
                    var paisSelecionado = await _context.Paises.FindAsync(viewModel.SelectedPaisId);
                    var regiaoParaBusca = (paisSelecionado?.CodigoIso == "PT") ? viewModel.SelectedRegiao : "N/A";

                    if (paisSelecionado?.CodigoIso == "PT" && string.IsNullOrWhiteSpace(regiaoParaBusca))
                    {
                        ModelState.AddModelError("SelectedRegiao", "Para Portugal, a região é obrigatória.");
                    }
                    else
                    {
                        var localidade = await _context.Localidades.FirstOrDefaultAsync(l =>
                            l.NomeLocalidade.ToLower() == viewModel.NomeLocalidade.ToLower() &&
                            l.PaisId == viewModel.SelectedPaisId &&
                            l.Regiao.ToLower() == regiaoParaBusca.ToLower());

                        if (localidade == null)
                        {
                            localidade = new Localidade
                            {
                                NomeLocalidade = viewModel.NomeLocalidade,
                                PaisId = viewModel.SelectedPaisId,
                                Regiao = regiaoParaBusca
                            };
                            _context.Localidades.Add(localidade);
                            await _context.SaveChangesAsync();
                        }

                        viewModel.Cliente.LocalidadeId = localidade.LocalidadeId;
                        _context.Clientes.Add(viewModel.Cliente);
                        await _context.SaveChangesAsync();
                        TempData["MensagemSucesso"] = $"Cliente '{viewModel.Cliente.Nome}' criado com sucesso!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "DbUpdateException ao criar cliente '{Nome}'. InnerException: {InnerMsg}", viewModel.Cliente.Nome, dbEx.InnerException?.Message);
                    ModelState.AddModelError(string.Empty, "Não foi possível guardar o cliente. Detalhe: " + (dbEx.InnerException?.Message ?? dbEx.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado ao criar cliente '{Nome}'.", viewModel.Cliente.Nome);
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro inesperado ao tentar guardar o cliente.");
                }
            }

            _logger.LogWarning("ModelState inválido ao tentar criar Cliente. Erros: {ModelStateErrorsJson}", JsonSerializer.Serialize(ModelState.Values.SelectMany(v => v.Errors)));

            viewModel.PaisesList = await _context.Paises
                .OrderBy(p => p.NomePais)
                .Select(p => new SelectListItem { Value = p.PaisId.ToString(), Text = p.NomePais })
                .ToListAsync();
            return View(viewModel);
        }

        // GET: Client/Edit/5
        public async Task<IActionResult> Edit(ulong? id)
        {
            if (id == null || id == 0) return NotFound();
            _logger.LogInformation("Acedendo a GET Client/Edit para ID: {ClienteId}", id);

            var cliente = await _context.Clientes
                            .Include(c => c.LocalidadeNavigation)
                            .ThenInclude(l => l.Pais)
                            .FirstOrDefaultAsync(c => c.ClienteId == id);

            if (cliente == null)
            {
                _logger.LogWarning("GET Client/Edit - Cliente com ID: {ClienteId} não encontrado.", id);
                TempData["MensagemErro"] = "Cliente não encontrado.";
                return NotFound();
            }

            var viewModel = new ClienteCreateViewModel { Cliente = cliente };
            try
            {
                viewModel.PaisesList = await _context.Paises
                    .OrderBy(p => p.NomePais)
                    .Select(p => new SelectListItem { Value = p.PaisId.ToString(), Text = p.NomePais })
                    .ToListAsync();

                if (cliente.LocalidadeNavigation?.PaisId != null)
                {
                    viewModel.SelectedPaisId = cliente.LocalidadeNavigation.PaisId;
                    if (cliente.LocalidadeNavigation.Pais.CodigoIso == "PT")
                    {
                        viewModel.SelectedRegiao = cliente.LocalidadeNavigation.Regiao;
                    }
                }

                viewModel.NomeLocalidade = cliente.LocalidadeNavigation?.NomeLocalidade ?? "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao popular o ViewModel em GET Client/Edit.");
                TempData["MensagemErro"] = "Ocorreu um erro ao carregar os dados para edição.";
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // POST: Client/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ulong id, ClienteCreateViewModel viewModel)
        {
            if (id != viewModel.Cliente.ClienteId)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(viewModel.Cliente) + "." + nameof(Models.Cliente.LocalidadeNavigation));

            if (ModelState.IsValid)
            {
                try
                {
                    var paisSelecionado = await _context.Paises.FindAsync(viewModel.SelectedPaisId);
                    var regiaoParaBusca = (paisSelecionado?.CodigoIso == "PT") ? viewModel.SelectedRegiao : "N/A";

                    if (paisSelecionado?.CodigoIso == "PT" && string.IsNullOrWhiteSpace(regiaoParaBusca))
                    {
                        ModelState.AddModelError("SelectedRegiao", "Para Portugal, a região é obrigatória.");
                    }
                    else
                    {
                        var localidade = await _context.Localidades.FirstOrDefaultAsync(l =>
                            l.NomeLocalidade.ToLower() == viewModel.NomeLocalidade.ToLower() &&
                            l.PaisId == viewModel.SelectedPaisId &&
                            l.Regiao.ToLower() == regiaoParaBusca.ToLower());

                        if (localidade == null)
                        {
                            localidade = new Localidade
                            {
                                NomeLocalidade = viewModel.NomeLocalidade,
                                PaisId = viewModel.SelectedPaisId,
                                Regiao = regiaoParaBusca
                            };
                            _context.Localidades.Add(localidade);
                            await _context.SaveChangesAsync();
                        }

                        viewModel.Cliente.LocalidadeId = localidade.LocalidadeId;
                        _context.Update(viewModel.Cliente);
                        await _context.SaveChangesAsync();
                        TempData["MensagemSucesso"] = $"Cliente '{viewModel.Cliente.Nome}' atualizado com sucesso!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ClienteExists(viewModel.Cliente.ClienteId)) { return NotFound(); } else { throw; }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado ao editar cliente '{Nome}'.", viewModel.Cliente.Nome);
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro inesperado ao tentar atualizar o cliente.");
                }
            }

            _logger.LogWarning("ModelState inválido ao tentar editar Cliente ID: {ClienteId}", viewModel.Cliente.ClienteId);
            viewModel.PaisesList = await _context.Paises
                .OrderBy(p => p.NomePais)
                .Select(p => new SelectListItem { Value = p.PaisId.ToString(), Text = p.NomePais })
                .ToListAsync();
            return View(viewModel);
        }

        // NOVA ACTION PARA AJAX: Obter Regiões de um País
        [HttpGet]
        public async Task<JsonResult> GetRegioesPorPais(ulong paisId)
        {
            _logger.LogInformation("GetRegioesPorPais chamada para PaisId: {PaisId}", paisId);
            try
            {
                var regioes = await _context.Localidades
                    .Where(l => l.PaisId == paisId && !string.IsNullOrEmpty(l.Regiao) && l.Regiao != "N/A")
                    .Select(l => l.Regiao)
                    .Distinct()
                    .OrderBy(r => r)
                    .ToListAsync();

                return Json(regioes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar regiões para o PaisId: {PaisId}", paisId);
                return Json(new List<string>());
            }
        }

        // GET: Client/Delete/5
        public async Task<IActionResult> Delete(ulong? id)
        {
            if (id == null || id == 0) return NotFound();
            _logger.LogInformation("Acedendo a GET Client/Delete para ID: {ClienteId}", id);
            try
            {
                var cliente = await _context.Clientes
                    .Include(c => c.LocalidadeNavigation)
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
            if (id == 0) return NotFound();
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
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DbUpdateException ao apagar cliente ID: {ClienteId}. InnerException: {InnerMsg}", id, dbEx.InnerException?.Message);
                TempData["MensagemErro"] = "Não foi possível apagar o cliente. Pode estar associado a outros registos.";
                return RedirectToAction(nameof(Delete), new { id = id });
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