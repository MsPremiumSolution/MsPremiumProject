using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MSPremiumProject.ViewModels;
using Microsoft.AspNetCore.Http;

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
        // ETAPA 1: PÁGINA DE SELEÇÃO DE CLIENTE
        //================================================================================
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["Title"] = "Novo Orçamento - Selecionar Cliente";
            ViewData["CurrentFilter"] = searchTerm;
            HttpContext.Session.Clear(); // Limpa qualquer orçamento antigo ao voltar para o início

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
        // ETAPA 2: INICIAR ORÇAMENTO E IR PARA ESCOLHA DA TIPOLOGIA
        //================================================================================
        [HttpGet]
        public async Task<IActionResult> CreateBudgetForClient(ulong id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                TempData["MensagemErro"] = "Cliente não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            HttpContext.Session.Clear();
            HttpContext.Session.SetString("CurrentBudget_ClienteId", id.ToString());
            HttpContext.Session.SetString("CurrentBudget_ClienteNome", $"{cliente.Nome} {cliente.Apelido}");

            return RedirectToAction(nameof(TipologiaConstrutiva));
        }

        //================================================================================
        // ETAPA 3: PÁGINA DE ESCOLHA DA TIPOLOGIA CONSTRUTIVA
        //================================================================================
        [HttpGet]
        public async Task<IActionResult> TipologiaConstrutiva()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("CurrentBudget_ClienteId")))
            {
                TempData["MensagemErro"] = "Sessão de orçamento inválida. Por favor, selecione o cliente novamente.";
                return RedirectToAction(nameof(Index));
            }

            var tipologiasDisponiveis = await _context.TipologiasConstrutivas.OrderBy(t => t.Nome).ToListAsync();
            return View(tipologiasDisponiveis);
        }

        //================================================================================
        // ETAPA 4: GUARDAR TIPOLOGIA E IR PARA ESCOLHA DO TRATAMENTO
        //================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveTipologiaAndContinue(ulong selectedTipologiaId)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("CurrentBudget_ClienteId")))
            {
                TempData["MensagemErro"] = "Sessão de orçamento expirada.";
                return RedirectToAction("Index");
            }

            if (selectedTipologiaId <= 0)
            {
                TempData["MensagemErro"] = "Por favor, selecione uma tipologia construtiva.";
                return RedirectToAction(nameof(TipologiaConstrutiva));
            }

            HttpContext.Session.SetString("CurrentBudget_TipologiaId", selectedTipologiaId.ToString());

            return RedirectToAction(nameof(SelectTreatment));
        }

        //================================================================================
        // ETAPA 5: PÁGINA DE ESCOLHA DO TIPO DE TRATAMENTO
        //================================================================================
        [HttpGet]
        public IActionResult SelectTreatment()
        {
            var clienteId = HttpContext.Session.GetString("CurrentBudget_ClienteId");
            var clienteNome = HttpContext.Session.GetString("CurrentBudget_ClienteNome");

            if (string.IsNullOrEmpty(clienteId))
            {
                TempData["MensagemErro"] = "Sessão de orçamento expirada.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new SelectTreatmentViewModel
            {
                ClienteId = ulong.Parse(clienteId),
                NomeCliente = clienteNome
            };

            return View(viewModel);
        }

        //================================================================================
        // ETAPA 6: PROCESSAR ESCOLHA DO TRATAMENTO E INICIAR FLUXO FINAL
        //================================================================================
        [HttpGet]
        public IActionResult Create(string treatmentType)
        {
            var clienteId = HttpContext.Session.GetString("CurrentBudget_ClienteId");
            if (string.IsNullOrEmpty(clienteId))
            {
                TempData["MensagemErro"] = "Sessão de orçamento expirada.";
                return RedirectToAction(nameof(Index));
            }

            HttpContext.Session.SetString("CurrentBudget_TreatmentType", treatmentType);

            if (treatmentType.Equals("QualidadeAr", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("ColecaoDados");
            }
            else if (treatmentType.Equals("Estrutural", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(nameof(EditTratamentoEstrutural));
            }
            else
            {
                TempData["MensagemErro"] = "Tipo de tratamento desconhecido.";
                return RedirectToAction(nameof(Index));
            }
        }

        //================================================================================
        // PÁGINAS PLACEHOLDER PARA OS FLUXOS FINAIS
        //================================================================================

        [HttpGet]
        public IActionResult ColecaoDados()
        {
            ViewData["Title"] = "Coleção de Dados";
            // Aqui irás construir o formulário para a coleção de dados
            return View();
        }

        [HttpGet]
        public IActionResult EditTratamentoEstrutural()
        {
            ViewData["Title"] = "Editar Tratamento Estrutural";
            return View();
        }
    }
}