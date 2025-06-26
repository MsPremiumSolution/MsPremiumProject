// File: Controllers/BudgetController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MSPremiumProject.Controllers
{
    [Authorize] // Adiciona autorização se esta página deve ser protegida
    public class BudgetController : Controller
    {
        private readonly AppDbContext _context;

        public BudgetController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Budget/Index ou Budget/Index?searchTerm=Maria
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["Title"] = "Novo Orçamento - Selecionar Cliente";
            ViewData["CurrentFilter"] = searchTerm;

            IQueryable<Cliente> clientesQuery = _context.Clientes
                                        .Include(c => c.LocalidadeNavigation)
                                        // .ThenInclude(l => l.Pais) // Descomente se precisar do país no partial view
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

            // Verifica se o pedido é AJAX. "X-Requested-With" é um header comum enviado por jQuery.
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ClientListPartial", clientes); // Retorna apenas a lista/tabela de clientes
            }

            // Para o carregamento inicial da página completa
            if (TempData["MensagemSucesso"] != null)
                ViewData["MensagemSucesso"] = TempData["MensagemSucesso"];
            if (TempData["MensagemErro"] != null)
                ViewData["MensagemErro"] = TempData["MensagemErro"];

            return View(clientes); // Retorna a view completa
        }

        // GET: Budget/CreateBudgetForClient/{clientId}
        public async Task<IActionResult> CreateBudgetForClient(ulong clientId)
        {
            var cliente = await _context.Clientes
                                .Include(c => c.LocalidadeNavigation)
                                .FirstOrDefaultAsync(c => c.ClienteId == clientId);

            if (cliente == null)
            {
                TempData["MensagemErro"] = "Cliente não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // TODO: Redirecionar para a página de criação de orçamento com os dados do cliente
            // Exemplo: return View("CreateBudgetForm", cliente); // Ou um ViewModel específico
            TempData["MensagemSucesso"] = $"Cliente '{cliente.Nome} {cliente.Apelido}' selecionado. (Próximo passo: formulário de orçamento)";
            // Por enquanto, vamos voltar para a lista, idealmente iria para o formulário.
            // return RedirectToAction("Index", "Client"); // Ou para uma página de criação de orçamento.
            return View("CreateBudgetForm", cliente); // Supondo que tens ou vais criar esta View.
        }
    }
}