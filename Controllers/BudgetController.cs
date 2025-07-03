using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MSPremiumProject.ViewModels; // Necessário para o SelectTreatmentViewModel
using Microsoft.AspNetCore.Http; // Necessário para usar HttpContext.Session

namespace MSPremiumProject.Controllers
{
    [Authorize]
    public class BudgetController : Controller
    {
        private readonly AppDbContext _context;

        public BudgetController(AppDbContext context)
        {
            _context = context;
        }

        //================================================================================
        // PASSO 1: PÁGINA DE SELEÇÃO DE CLIENTE (O teu código original, está perfeito)
        //================================================================================
        // GET: /Budget ou /Budget/Index
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["Title"] = "Novo Orçamento - Selecionar Cliente";
            ViewData["CurrentFilter"] = searchTerm;

            IQueryable<Cliente> clientesQuery = _context.Clientes
                                        .Include(c => c.LocalidadeNavigation)
                                        .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                string searchTermLower = searchTerm.ToLower();
                clientesQuery = clientesQuery.Where(c =>
                    c.Nome.ToLower().Contains(searchTermLower) ||
                    (c.Apelido != null && c.Apelido.ToLower().Contains(searchTermLower))
                );
            }

            var clientes = await clientesQuery.OrderBy(c => c.Nome).ThenBy(c => c.Apelido).ToListAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ClientListPartial", clientes);
            }

            return View(clientes);
        }

        //================================================================================
        // PASSO 2: MOSTRAR A PÁGINA DE ESCOLHA DE TRATAMENTO (Estrutural ou Qualidade do Ar)
        //================================================================================
        // GET: /Budget/CreateBudgetForClient/5
        [HttpGet]
        public async Task<IActionResult> CreateBudgetForClient(ulong id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                TempData["MensagemErro"] = "Cliente não encontrado. Por favor, tente novamente.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new SelectTreatmentViewModel
            {
                ClienteId = cliente.ClienteId,
                NomeCliente = $"{cliente.Nome} {cliente.Apelido}"
            };

            // Mostra a página com os dois cartões de escolha
            return View("SelectTreatment", viewModel);
        }

        //================================================================================
        // PASSO 3: PROCESSAR A ESCOLHA DO TRATAMENTO E REDIRECIONAR
        //================================================================================
        // GET: /Budget/Create?clienteId=5&treatmentType=QualidadeAr
        [HttpGet]
        public async Task<IActionResult> Create(ulong clienteId, string treatmentType)
        {
            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null)
            {
                TempData["MensagemErro"] = "Cliente não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Guarda as informações essenciais na Sessão para usar nos próximos passos
            HttpContext.Session.SetString("CurrentBudget_TreatmentType", treatmentType);
            HttpContext.Session.SetString("CurrentBudget_ClienteId", clienteId.ToString());
            HttpContext.Session.SetString("CurrentBudget_ClienteNome", $"{cliente.Nome} {cliente.Apelido}");


            if (treatmentType.Equals("QualidadeAr", StringComparison.OrdinalIgnoreCase))
            {
                // Inicia o fluxo de "Qualidade do Ar", redirecionando para a primeira etapa do submenu
                return RedirectToAction(nameof(TipologiaConstrutiva));
            }
            else if (treatmentType.Equals("Estrutural", StringComparison.OrdinalIgnoreCase))
            {
                // Inicia o fluxo de "Tratamento Estrutural"
                // (Por agora, podemos redirecionar para uma action placeholder)
                return RedirectToAction(nameof(EditTratamentoEstrutural));
            }
            else
            {
                TempData["MensagemErro"] = "Tipo de tratamento desconhecido.";
                return RedirectToAction(nameof(Index));
            }
        }

        //================================================================================
        // PASSO 4: PÁGINAS DO SUBMENU "QUALIDADE DO AR"
        //================================================================================

        // GET: /Budget/TipologiaConstrutiva
        [HttpGet]
        public IActionResult TipologiaConstrutiva()
        {
            // Verifica se a sessão de orçamento ainda está ativa
            var clienteId = HttpContext.Session.GetString("CurrentBudget_ClienteId");
            if (string.IsNullOrEmpty(clienteId))
            {
                TempData["MensagemErro"] = "Sessão de orçamento expirada. Por favor, selecione o cliente novamente.";
                return RedirectToAction(nameof(Index));
            }

            // Futuramente, podes passar dados para a view se necessário (ex: uma lista de tipologias da BD)
            // Por agora, apenas mostra a view estática
            return View();
        }

        // Adicione aqui as outras actions do submenu (ColecaoDados, Objetivos, etc.) quando as criares
        // [HttpGet]
        // public IActionResult ColecaoDados() { ... }


        //================================================================================
        // PLACEHOLDER PARA O FLUXO DE TRATAMENTO ESTRUTURAL
        //================================================================================
        [HttpGet]
        public IActionResult EditTratamentoEstrutural()
        {
            ViewData["Title"] = "Editar Tratamento Estrutural";
            var clienteNome = HttpContext.Session.GetString("CurrentBudget_ClienteNome");
            ViewData["Message"] = $"Esta é a página de edição para o tratamento estrutural do cliente {clienteNome}.";
            return View();
        }
    }
}