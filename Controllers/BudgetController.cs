using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MSPremiumProject.ViewModels; // <<---- 1. ADICIONAR ESTA DIRECTIVA para encontrar o SelectTreatmentViewModel

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

        // GET: Budget/Index ou Budget/Index?searchTerm=Maria
        // Esta action já está ótima, não precisa de alterações.
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

            if (TempData["MensagemSucesso"] != null)
                ViewData["MensagemSucesso"] = TempData["MensagemSucesso"];
            if (TempData["MensagemErro"] != null)
                ViewData["MensagemErro"] = TempData["MensagemErro"];

            return View(clientes);
        }

        // <<---- 2. CORREÇÃO PRINCIPAL ESTÁ AQUI ---->>
        // GET: Budget/CreateBudgetForClient/5
        // Renomeei o parâmetro de 'clientId' para 'id' para corresponder à configuração de rota padrão do ASP.NET Core (ex: {controller}/{action}/{id?})
        // Se a tua rota estiver configurada de outra forma, podes manter 'clientId'. 'id' é mais comum.
        [HttpGet]
        public async Task<IActionResult> CreateBudgetForClient(ulong id)
        {
            // Procura o cliente na base de dados pelo ID recebido.
            var cliente = await _context.Clientes.FindAsync(id);

            // Se o cliente não for encontrado, mostra uma mensagem de erro e volta para a lista.
            if (cliente == null)
            {
                TempData["MensagemErro"] = "Cliente não encontrado. Por favor, tente novamente.";
                return RedirectToAction(nameof(Index)); // Volta para a lista de clientes
            }

            // Prepara o ViewModel com os dados que a view de seleção de tratamento precisa.
            var viewModel = new SelectTreatmentViewModel
            {
                ClienteId = cliente.ClienteId,
                NomeCliente = $"{cliente.Nome} {cliente.Apelido}"
            };

            // Chama a view "SelectTreatment.cshtml" e envia-lhe os dados através do ViewModel.
            return View("SelectTreatment", viewModel);
        }
    }
}