// File: Controllers/BudgetController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Se precisares de autorização

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
            ViewData["CurrentFilter"] = searchTerm; // Para manter o termo na caixa de pesquisa

            var clientesQuery = _context.Clientes
                                        .Include(c => c.LocalidadeNavigation) // Para mostrar a localidade
                                            .ThenInclude(l => l.Pais) // Se quiseres mostrar o país também
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

            // Verifica se TempData tem mensagens e passa-as para ViewData ou diretamente para a View
            // (Opcional, mas bom se quiseres manter o sistema de mensagens)
            if (TempData["MensagemSucesso"] != null)
                ViewData["MensagemSucesso"] = TempData["MensagemSucesso"];
            if (TempData["MensagemErro"] != null)
                ViewData["MensagemErro"] = TempData["MensagemErro"];


            return View(clientes);
        }

        // GET: Budget/CreateBudgetForClient/{clientId}
        // Esta ação será chamada quando clicares em "Criar Orçamento" para um cliente
        // No futuro, aqui irás preparar o modelo para a página de criação do orçamento.
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

            // TODO: Aqui irias criar um ViewModel para o orçamento,
            // preencher com dados do cliente e passar para uma nova View "CreateBudget.cshtml"
            // Por agora, vamos apenas mostrar uma view simples de confirmação ou detalhes do cliente.

            ViewData["Title"] = $"Criar Orçamento para {cliente.Nome} {cliente.Apelido}";
            // Poderias criar um ViewModel específico aqui:
            // var budgetViewModel = new CreateBudgetViewModel { Cliente = cliente };
            // return View("CreateBudgetForm", budgetViewModel); // Uma nova view para o formulário do orçamento

            // Por simplicidade, vamos redirecionar para uma página de detalhes do cliente (exemplo)
            // ou apenas retornar os dados do cliente para uma view de "pré-orçamento"
            // Para este exemplo, vou assumir que tens uma view "CreateBudget.cshtml" que recebe o Cliente.
            TempData["MensagemSucesso"] = $"A preparar orçamento para o cliente: {cliente.Nome} {cliente.Apelido}. (ID: {cliente.ClienteId})";
            //return View("CreateBudgetForm", cliente); // Se tiveres uma View CreateBudgetForm.cshtml que aceita um Cliente
            return RedirectToAction("Index", "Client", new { area = "" }); // Redireciona de volta para a lista de clientes por agora
            // Ou, melhor, redireciona para uma futura página de criação de orçamento:
            // return RedirectToAction("New", "Orcamento", new { clienteId = clientId });
        }
    }
}