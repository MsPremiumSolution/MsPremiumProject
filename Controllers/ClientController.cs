// Ficheiro: Controllers/ClientController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSPremiumProject.Data;
using MSPremiumProject.Models;
using MSPremiumProject.ViewModels;
using MSPremiumProject.Utils; // Supondo que o seu validador de NIF está aqui
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSPremiumProject.Controllers
{
    // [Authorize] // Descomente para proteger o controller
    public class ClientController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClientController> _logger;

        public ClientController(AppDbContext context, ILogger<ClientController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Client
        public async Task<IActionResult> Index()
        {
            var clientes = await _context.Clientes
                                     .Include(c => c.LocalidadeNavigation.Pais)
                                     .OrderBy(c => c.Nome).ThenBy(c => c.Apelido)
                                     .AsNoTracking()
                                     .ToListAsync();
            return View(clientes);
        }

        // GET: Client/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new ClienteCreateViewModel();
            await PopulateViewModelDropdowns(viewModel);
            return View(viewModel);
        }

        // POST: Client/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateViewModelDropdowns(viewModel);
                return View(viewModel);
            }

            // --- LÓGICA DE VALIDAÇÃO E CRIAÇÃO NO CONTROLLER ---

            var localidade = await FindOrCreateLocalidade(viewModel.NomeLocalidade, viewModel.SelectedPaisId);
            var pais = await _context.Paises.FindAsync(localidade.PaisId);
            string? paisCodigoIso = pais?.CodigoIso;

            // Validações personalizadas...
            ValidateClienteData(viewModel, paisCodigoIso);

            if (!ModelState.IsValid)
            {
                await PopulateViewModelDropdowns(viewModel);
                return View(viewModel);
            }

            try
            {
                var clienteFinal = viewModel.Cliente;
                clienteFinal.LocalidadeId = localidade.LocalidadeId;

                _context.Clientes.Add(clienteFinal);
                await _context.SaveChangesAsync();

                TempData["MensagemSucesso"] = $"Cliente '{clienteFinal.Nome}' criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro ao salvar o cliente.");
                ModelState.AddModelError(string.Empty, "Ocorreu um erro ao salvar os dados.");
                await PopulateViewModelDropdowns(viewModel);
                return View(viewModel);
            }
        }

        // GET: Client/Edit/5
        public async Task<IActionResult> Edit(ulong? id)
        {
            if (id == null) return NotFound();
            var cliente = await _context.Clientes
                .Include(c => c.LocalidadeNavigation.Pais)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClienteId == id);

            if (cliente == null) return NotFound();

            var viewModel = new ClienteCreateViewModel { Cliente = cliente };
            await PopulateViewModelDropdowns(viewModel, cliente.LocalidadeNavigation);

            // Pré-seleciona a localidade no ViewModel
            if (cliente.LocalidadeNavigation != null)
            {
                viewModel.NomeLocalidade = cliente.LocalidadeNavigation.Regiao; // Usa a propriedade 'Regiao'
            }

            return View(viewModel);
        }

        // POST: Client/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ulong id, ClienteCreateViewModel viewModel)
        {
            if (id != viewModel.Cliente.ClienteId) return BadRequest();
            if (!ModelState.IsValid)
            {
                await PopulateViewModelDropdowns(viewModel);
                return View(viewModel);
            }

            var localidade = await FindOrCreateLocalidade(viewModel.NomeLocalidade, viewModel.SelectedPaisId);
            var pais = await _context.Paises.FindAsync(localidade.PaisId);
            string? paisCodigoIso = pais?.CodigoIso;

            ValidateClienteData(viewModel, paisCodigoIso);

            if (!ModelState.IsValid)
            {
                await PopulateViewModelDropdowns(viewModel);
                return View(viewModel);
            }

            try
            {
                viewModel.Cliente.LocalidadeId = localidade.LocalidadeId;
                _context.Update(viewModel.Cliente);
                await _context.SaveChangesAsync();
                TempData["MensagemSucesso"] = "Cliente atualizado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ClienteExists(viewModel.Cliente.ClienteId)) return NotFound();
                else throw;
            }

            await PopulateViewModelDropdowns(viewModel);
            return View(viewModel);
        }

        // Método privado para encontrar ou criar a Localidade (simplificado)
        private async Task<Localidade> FindOrCreateLocalidade(string nomeLocalidade, ulong paisId)
        {
            // A busca agora é só pelo nome (Regiao) e PaisId
            var localidade = await _context.Localidades.FirstOrDefaultAsync(l =>
                l.Regiao.ToLower() == nomeLocalidade.ToLower() &&
                l.PaisId == paisId);

            if (localidade == null)
            {
                localidade = new Localidade
                {
                    Regiao = nomeLocalidade.Trim(), // O nome da localidade vai para a propriedade 'Regiao'
                    PaisId = paisId
                };
                _context.Localidades.Add(localidade);
                await _context.SaveChangesAsync();
            }
            return localidade;
        }

        // Método privado para as validações personalizadas
        private void ValidateClienteData(ClienteCreateViewModel viewModel, string? paisCodigoIso)
        {
            // Validação do NIF
            if (!string.IsNullOrWhiteSpace(viewModel.Cliente.NumeroFiscal))
            {
                if (string.IsNullOrWhiteSpace(paisCodigoIso))
                    ModelState.AddModelError("Cliente.NumeroFiscal", "País não encontrado para validar NIF.");
                else if (!EuropeanNifValidator.ValidateNif(paisCodigoIso, viewModel.Cliente.NumeroFiscal))
                    ModelState.AddModelError("Cliente.NumeroFiscal", $"O NIF não é válido para o país selecionado.");
            }

            // Validação do Código Postal
            if (paisCodigoIso == "PT")
            {
                if (string.IsNullOrWhiteSpace(viewModel.Cliente.Cp4)) ModelState.AddModelError("Cliente.Cp4", "Obrigatório para Portugal.");
                if (string.IsNullOrWhiteSpace(viewModel.Cliente.Cp3)) ModelState.AddModelError("Cliente.Cp3", "Obrigatório para Portugal.");
            }
            else if (!string.IsNullOrWhiteSpace(paisCodigoIso))
            {
                if (string.IsNullOrWhiteSpace(viewModel.Cliente.CodigoPostalEstrangeiro))
                    ModelState.AddModelError("Cliente.CodigoPostalEstrangeiro", "Obrigatório para países estrangeiros.");
            }
        }

        // Método privado para popular as dropdowns
        private async Task PopulateViewModelDropdowns(ClienteCreateViewModel viewModel, Localidade? localidadeAtual = null)
        {
            viewModel.PaisesList = new SelectList(await _context.Paises.OrderBy(p => p.NomePais).ToListAsync(), "PaisId", "NomePais", localidadeAtual?.PaisId);
            if (localidadeAtual != null)
            {
                viewModel.SelectedPaisId = localidadeAtual.PaisId;
                viewModel.NomeLocalidade = localidadeAtual.Regiao; // Usa a propriedade 'Regiao'
            }
        }

        // Outras ações (Details, Delete, GetRegioes, etc.)
        private async Task<bool> ClienteExists(ulong id)
        {
            return await _context.Clientes.AnyAsync(e => e.ClienteId == id);
        }

        [HttpGet] // É uma requisição GET feita via AJAX
        public async Task<IActionResult> GetDistritosPorPais(ulong paisId)
        {
            _logger.LogInformation("Recebido pedido AJAX para GetDistritosPorPais para PaisId: {PaisId}", paisId);

            try
            {
                // Busca as localidades (distritos) associadas ao PaísId fornecido
                // Ordena por nome para uma melhor experiência do utilizador
                var distritos = await _context.Localidades
                                              .Where(l => l.PaisId == paisId)
                                              .OrderBy(l => l.Regiao) // Ordenar pelo nome da Região
                                              .Select(l => new { value = l.LocalidadeId, text = l.Regiao })
                                              .AsNoTracking() // Boa prática para dados de somente leitura
                                              .ToListAsync();

                _logger.LogInformation("Encontrados {Count} distritos para o PaísId: {PaisId}", distritos.Count, paisId);

                // Retorna os dados como JSON. O JavaScript espera um array de objetos { value: ..., text: ... }
                return Json(distritos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter distritos para o PaísId: {PaisId}", paisId);
                // Em caso de erro, pode retornar um array vazio ou um status de erro.
                // Para o JavaScript atual, um array vazio fará a dropdown mostrar "Nenhum distrito disponível".
                return StatusCode(500, "Erro interno do servidor ao carregar distritos.");
                // Ou simplesmente: return Json(new List<object>());
            }
        }

        // ... (Seus outros métodos FindOrCreateLocalidade, ValidateClienteData, PopulateViewModelDropdowns, ClienteExists) ...
    }
}
