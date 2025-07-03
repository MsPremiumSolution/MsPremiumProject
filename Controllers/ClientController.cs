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

        // GET: Client (Lista de Clientes) - CORRIGIDO
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

        // GET: Client/Details/5 - CORRIGIDO
        public async Task<IActionResult> Details(ulong? id)
        {
            if (id == null || id == 0) return NotFound();
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
                    return NotFound();
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar detalhes do cliente ID: {ClienteId}", id);
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Client/Create - CORRIGIDO
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Acedendo a GET Client/Create.");
            var viewModel = new ClienteCreateViewModel();
            try
            {
                viewModel.PaisesList = await _context.Paises
                    .OrderBy(p => p.NomePais)
                    .Select(p => new SelectListItem { Value = p.PaisId.ToString(), Text = p.NomePais })
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
            }
            return View(viewModel);
        }

        // POST: Client/Create - ATUALIZADO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteCreateViewModel viewModel)
        {
            _logger.LogInformation("Acedendo a POST Client/Create.");

            ModelState.Remove(nameof(viewModel.Cliente.LocalidadeNavigation));

            // 1. Obter o país selecionado
            // Esta chamada é feita ANTES do ModelState.IsValid para que o PaisSelecionado esteja disponível
            // para a lógica de atribuição da LocalidadeId e o IValidatableObject.
            var paisSelecionado = await _context.Paises.FindAsync(viewModel.SelectedPaisId);

            // 2. Pré-processar a LocalidadeId e NomeLocalidadeTexto antes da validação do ModelState
            if (viewModel.SelectedPaisId > 0)
            {
                string localidadeLookupName = "";

                // Prioriza o valor do dropdown de SelectedRegiao para encontrar/criar a Localidade
                // Se SelectedRegiao estiver vazio, usa o NomeLocalidadeTexto digitado pelo usuário
                if (!string.IsNullOrWhiteSpace(viewModel.SelectedRegiao))
                {
                    localidadeLookupName = viewModel.SelectedRegiao;
                }
                else if (!string.IsNullOrWhiteSpace(viewModel.Cliente.NomeLocalidadeTexto))
                {
                    localidadeLookupName = viewModel.Cliente.NomeLocalidadeTexto;
                }

                // Se uma localidade foi determinada, tenta encontrá-la/criá-la na tabela Localidades
                if (!string.IsNullOrWhiteSpace(localidadeLookupName))
                {
                    var localidade = await _context.Localidades.FirstOrDefaultAsync(l =>
                        l.Regiao.ToLower() == localidadeLookupName.ToLower() &&
                        l.PaisId == viewModel.SelectedPaisId);

                    if (localidade == null)
                    {
                        localidade = new Localidade
                        {
                            Regiao = localidadeLookupName,
                            PaisId = viewModel.SelectedPaisId
                        };
                        _context.Localidades.Add(localidade);
                        await _context.SaveChangesAsync(); // SALVA A LOCALIDADE PARA OBTER O ID
                    }
                    viewModel.Cliente.LocalidadeId = localidade.LocalidadeId; // Atribui o ID ao cliente
                }
                else
                {
                    // Se não conseguiu determinar a localidadeLookupName, e LocalidadeId é obrigatória, adiciona um erro
                    if (viewModel.Cliente.LocalidadeId == 0)
                    {
                        // Este erro pode ser redundante se Cliente.NomeLocalidadeTexto já tem [Required] e é validado no ViewModel
                        ModelState.AddModelError(nameof(viewModel.Cliente.LocalidadeId), "Não foi possível determinar uma localidade válida para associação.");
                    }
                }
            }
            else
            {
                // Se o país não foi selecionado, garante que LocalidadeId não seja 0
                viewModel.Cliente.LocalidadeId = 0;
            }

            // A lógica de IValidatableObject da ViewModel será executada quando ModelState.IsValid for chamado.
            // O campo Cliente.NomeLocalidadeTexto já foi preenchido pelo Model Binder, ou pela lógica acima
            // se uma região foi selecionada.

            // 3. Valida o ModelState. Agora, Cliente.LocalidadeId já tem um valor, e NomeLocalidadeTexto está preenchido.
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Clientes.Add(viewModel.Cliente);
                    await _context.SaveChangesAsync();

                    TempData["MensagemSucesso"] = $"Cliente '{viewModel.Cliente.Nome}' criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado ao criar cliente.");
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro inesperado ao guardar o cliente.");
                }
            }

            _logger.LogWarning("ModelState inválido. Erros: {Errors}", JsonSerializer.Serialize(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

            viewModel.PaisesList = await _context.Paises
                .OrderBy(p => p.NomePais)
                .Select(p => new SelectListItem { Value = p.PaisId.ToString(), Text = p.NomePais })
                .ToListAsync();

            return View(viewModel);
        }

        // GET: Client/Edit/5 - CORRIGIDO
        public async Task<IActionResult> Edit(ulong? id)
        {
            if (id == null) return NotFound();
            _logger.LogInformation("Acedendo a GET Client/Edit para ID: {ClienteId}", id);

            var cliente = await _context.Clientes
                            .Include(c => c.LocalidadeNavigation)
                                .ThenInclude(l => l.Pais)
                            .FirstOrDefaultAsync(c => c.ClienteId == id);

            if (cliente == null) return NotFound();

            var viewModel = new ClienteCreateViewModel { Cliente = cliente };
            try
            {
                viewModel.PaisesList = await _context.Paises.OrderBy(p => p.NomePais)
                    .Select(p => new SelectListItem { Value = p.PaisId.ToString(), Text = p.NomePais }).ToListAsync();

                if (cliente.LocalidadeNavigation != null)
                {
                    viewModel.SelectedPaisId = cliente.LocalidadeNavigation.PaisId;
                    // O SelectedRegiao deve ser preenchido com a Regiao da LocalidadeNavigation
                    viewModel.SelectedRegiao = cliente.LocalidadeNavigation.Regiao;
                    // O Cliente.NomeLocalidadeTexto já estará preenchido pelo Model Binder ao atribuir viewModel.Cliente = cliente
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao popular ViewModel em GET Edit.");
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // POST: Client/Edit/5 - ATUALIZADO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ulong id, ClienteCreateViewModel viewModel)
        {
            if (id != viewModel.Cliente.ClienteId) return NotFound();
            _logger.LogInformation("Acedendo a POST Client/Edit para ID: {ClienteId}", id);

            ModelState.Remove(nameof(viewModel.Cliente.LocalidadeNavigation));

            // 1. Obter o país selecionado
            var paisSelecionado = await _context.Paises.FindAsync(viewModel.SelectedPaisId);

            // 2. Pré-processar a LocalidadeId e NomeLocalidadeTexto antes da validação do ModelState
            if (viewModel.SelectedPaisId > 0)
            {
                string localidadeLookupName = "";

                if (!string.IsNullOrWhiteSpace(viewModel.SelectedRegiao))
                {
                    localidadeLookupName = viewModel.SelectedRegiao;
                }
                else if (!string.IsNullOrWhiteSpace(viewModel.Cliente.NomeLocalidadeTexto))
                {
                    localidadeLookupName = viewModel.Cliente.NomeLocalidadeTexto;
                }

                if (!string.IsNullOrWhiteSpace(localidadeLookupName))
                {
                    var localidade = await _context.Localidades.FirstOrDefaultAsync(l =>
                        l.Regiao.ToLower() == localidadeLookupName.ToLower() &&
                        l.PaisId == viewModel.SelectedPaisId);

                    if (localidade == null)
                    {
                        localidade = new Localidade
                        {
                            Regiao = localidadeLookupName,
                            PaisId = viewModel.SelectedPaisId
                        };
                        _context.Localidades.Add(localidade);
                        await _context.SaveChangesAsync();
                    }
                    viewModel.Cliente.LocalidadeId = localidade.LocalidadeId;
                }
                else
                {
                    if (viewModel.Cliente.LocalidadeId == 0)
                    {
                        ModelState.AddModelError(nameof(viewModel.Cliente.LocalidadeId), "Não foi possível determinar uma localidade válida para associação.");
                    }
                }
            }
            else
            {
                viewModel.Cliente.LocalidadeId = 0;
            }

            // 3. Valida o ModelState.
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewModel.Cliente);
                    await _context.SaveChangesAsync();

                    TempData["MensagemSucesso"] = $"Cliente '{viewModel.Cliente.Nome}' atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ClienteExists(viewModel.Cliente.ClienteId)) { return NotFound(); } else { throw; }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado ao editar cliente.");
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro inesperado.");
                }
            }

            _logger.LogWarning("ModelState inválido ao tentar editar Cliente ID: {ClienteId}. Erros: {Errors}", viewModel.Cliente.ClienteId, JsonSerializer.Serialize(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

            viewModel.PaisesList = await _context.Paises.OrderBy(p => p.NomePais)
                .Select(p => new SelectListItem { Value = p.PaisId.ToString(), Text = p.NomePais }).ToListAsync();

            return View(viewModel);
        }

        // AJAX ACTION: Obter Regiões de um País - CORRIGIDO
        [HttpGet]
        public async Task<JsonResult> GetRegioesPorPais(ulong paisId)
        {
            _logger.LogInformation("GetRegioesPorPais chamada para PaisId: {PaisId}", paisId);
            try
            {
                var regioes = await _context.Localidades
                    .Where(l => l.PaisId == paisId && !string.IsNullOrEmpty(l.Regiao))
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
            if (id == null) return NotFound();
            _logger.LogInformation("Acedendo a GET Client/Delete para ID: {ClienteId}", id);
            try
            {
                var cliente = await _context.Clientes
                    .Include(c => c.LocalidadeNavigation)
                        .ThenInclude(l => l.Pais)
                    .FirstOrDefaultAsync(m => m.ClienteId == id);
                if (cliente == null)
                {
                    return NotFound();
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cliente para apagar, ID: {ClienteId}", id);
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ulong id)
        {
            _logger.LogInformation("Acedendo a POST Client/DeleteConfirmed para ID: {ClienteId}", id);
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente != null)
                {
                    _context.Clientes.Remove(cliente);
                    await _context.SaveChangesAsync();
                    TempData["MensagemSucesso"] = "Cliente apagado com sucesso.";
                }
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DbUpdateException ao apagar cliente ID: {ClienteId}.", id);
                TempData["MensagemErro"] = "Não foi possível apagar o cliente. Pode estar associado a outros registos.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao apagar cliente ID: {ClienteId}.", id);
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ClienteExists(ulong id)
        {
            return await _context.Clientes.AnyAsync(e => e.ClienteId == id);
        }
    }
}