using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Models;
using MSPremiumProject.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MSPremiumProject.Controllers
{
    [Authorize]
    public class BudgetController : Controller
    {
        private readonly AppDbContext _context;

        private const int ESTADO_EM_CURSO = 1;
        private const int ESTADO_CONCLUIDO = 2;

        public BudgetController(AppDbContext context)
        {
            _context = context;
        }

        //================================================================================
        // PÁGINA PRINCIPAL: LISTAGEM DE ORÇAMENTOS POR CONCLUIR
        //================================================================================
        [HttpGet]
        public async Task<IActionResult> OrçamentosEmCurso()
        {
            ViewData["Title"] = "Orçamentos por Concluir";
            var propostasEmCurso = await _context.Proposta
                                         .Where(p => p.EstadoPropostaId == ESTADO_EM_CURSO)
                                         .Include(p => p.Cliente)
                                         .Include(p => p.Estado)
                                         .OrderByDescending(p => p.DataProposta)
                                         .ToListAsync();
            return View(propostasEmCurso);
        }


        //================================================================================
        // ETAPA 1: PÁGINA DE SELEÇÃO DE CLIENTE (para criar um NOVO orçamento)
        //================================================================================
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["Title"] = "Novo Orçamento - Selecionar Cliente";
            ViewData["CurrentFilter"] = searchTerm;
            HttpContext.Session.Clear();

            IQueryable<Cliente> clientesQuery = _context.Clientes.Include(c => c.LocalidadeNavigation).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                string searchTermLower = searchTerm.ToLower();
                clientesQuery = clientesQuery.Where(c => c.Nome.ToLower().Contains(searchTermLower) || (c.Apelido != null && c.Apelido.ToLower().Contains(searchTermLower)));
            }

            var clientes = await clientesQuery.OrderBy(c => c.Nome).ThenBy(c => c.Apelido).ToListAsync();
            return View(clientes);
        }

        //================================================================================
        // ETAPA 2: INICIAR ORÇAMENTO (CRIAR PROPOSTA) OU CONTINUAR UM EXISTENTE
        //================================================================================
        [HttpGet]
        public async Task<IActionResult> IniciarOrcamento(ulong clienteId)
        {
            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null)
            {
                TempData["MensagemErro"] = "Cliente não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var utilizadorId = ulong.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var novaProposta = new Proposta
            {
                ClienteId = clienteId,
                UtilizadorId = utilizadorId,
                EstadoPropostaId = ESTADO_EM_CURSO,
                DataProposta = DateTime.UtcNow
            };

            _context.Proposta.Add(novaProposta);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("CurrentPropostaId", novaProposta.PropostaId.ToString());

            // Iniciar o fluxo de Qualidade do Ar
            ViewData["CurrentBudgetContext"] = "QualidadeDoAr"; // <<< Adicionado aqui
            return RedirectToAction(nameof(TipologiaConstrutiva));
        }

        [HttpGet]
        public async Task<IActionResult> ContinuarOrcamento(ulong id)
        {
            var proposta = await _context.Proposta.FindAsync(id);
            if (proposta == null || proposta.EstadoPropostaId != ESTADO_EM_CURSO)
            {
                TempData["MensagemErro"] = "Orçamento inválido ou já concluído.";
                return RedirectToAction(nameof(OrçamentosEmCurso));
            }

            HttpContext.Session.SetString("CurrentPropostaId", proposta.PropostaId.ToString());

            // Lógica para decidir para onde redirecionar e definir o contexto do submenu
            if (proposta.QualidadeDoArId.HasValue) // Se já tem um tratamento de Qualidade do Ar
            {
                ViewData["CurrentBudgetContext"] = "QualidadeDoAr"; // <<< Adicionado aqui
                return RedirectToAction("EditQualidadeDoAr", new { id = proposta.QualidadeDoArId });
            }
            // Add other treatment types here, e.g., if (proposta.TratamentoEstruturalId.HasValue) { ... }

            // Se não tiver nenhum tratamento definido, ou se for para a tipologia
            ViewData["CurrentBudgetContext"] = "QualidadeDoAr"; // <<< Adicionado aqui
            if (proposta.TipologiaConstrutivaId == null)
            {
                return RedirectToAction(nameof(TipologiaConstrutiva));
            }
            if (proposta.QualidadeDoArId == null)
            {
                return RedirectToAction(nameof(SelectTreatment));
            }

            // Fallback: se não souber, vai para a primeira etapa do fluxo de QA
            return RedirectToAction(nameof(TipologiaConstrutiva));
        }

        //================================================================================
        // ETAPA 3: PÁGINA DE ESCOLHA DA TIPOLOGIA CONSTRUTIVA
        //================================================================================
        [HttpGet]
        public async Task<IActionResult> TipologiaConstrutiva()
        {
            if (!ulong.TryParse(HttpContext.Session.GetString("CurrentPropostaId"), out ulong propostaId))
            {
                TempData["MensagemErro"] = "Sessão de orçamento inválida.";
                return RedirectToAction(nameof(OrçamentosEmCurso));
            }

            var proposta = await _context.Proposta.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.PropostaId == propostaId);
            if (proposta == null)
            {
                TempData["MensagemErro"] = "Orçamento não encontrado.";
                return RedirectToAction(nameof(OrçamentosEmCurso));
            }

            ViewData["CurrentBudgetContext"] = "QualidadeDoAr"; // <<< Adicionado aqui
            ViewData["ClienteNome"] = $"{proposta.Cliente.Nome} {proposta.Cliente.Apelido}";
            ViewData["SelectedTipologiaId"] = proposta.TipologiaConstrutivaId;

            var tipologiasDisponiveis = await _context.TipologiasConstrutivas.OrderBy(t => t.Nome).ToListAsync();
            return View(tipologiasDisponiveis);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveTipologiaAndContinue(ulong selectedTipologiaId)
        {
            if (!ulong.TryParse(HttpContext.Session.GetString("CurrentPropostaId"), out ulong propostaId))
            {
                TempData["MensagemErro"] = "Sessão de orçamento expirada.";
                return RedirectToAction(nameof(OrçamentosEmCurso));
            }
            if (selectedTipologiaId <= 0)
            {
                TempData["MensagemErro"] = "Por favor, selecione uma tipologia construtiva.";
                return RedirectToAction(nameof(TipologiaConstrutiva));
            }

            var proposta = await _context.Proposta.FindAsync(propostaId);
            if (proposta == null) return NotFound();

            proposta.TipologiaConstrutivaId = selectedTipologiaId;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(SelectTreatment));
        }

        //================================================================================
        // ETAPA 4: PÁGINA DE ESCOLHA DO TIPO DE TRATAMENTO
        //================================================================================
        [HttpGet]
        public async Task<IActionResult> SelectTreatment()
        {
            if (!ulong.TryParse(HttpContext.Session.GetString("CurrentPropostaId"), out ulong propostaId))
            {
                TempData["MensagemErro"] = "Sessão de orçamento expirada.";
                return RedirectToAction(nameof(OrçamentosEmCurso));
            }

            var proposta = await _context.Proposta.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.PropostaId == propostaId);
            if (proposta == null) return NotFound();

            ViewData["CurrentBudgetContext"] = "QualidadeDoAr"; // <<< Adicionado aqui
            var viewModel = new SelectTreatmentViewModel
            {
                NomeCliente = $"{proposta.Cliente.Nome} {proposta.Cliente.Apelido}"
            };

            return View(viewModel);
        }

        //================================================================================
        // ETAPA 5: PROCESSAR ESCOLHA DO TRATAMENTO E INICIAR FLUXO DE EDIÇÃO
        //================================================================================
        [HttpGet]
        public async Task<IActionResult> ProcessTreatmentSelection(string treatmentType)
        {
            if (!ulong.TryParse(HttpContext.Session.GetString("CurrentPropostaId"), out ulong propostaId))
            {
                TempData["MensagemErro"] = "Sessão de orçamento expirada.";
                return RedirectToAction(nameof(OrçamentosEmCurso));
            }

            var proposta = await _context.Proposta.FindAsync(propostaId);
            if (proposta == null) return NotFound();

            if (proposta.QualidadeDoArId.HasValue)
            {
                TempData["MensagemAviso"] = "Esta proposta já tem um tipo de tratamento associado. A continuar edição...";
                // Redireciona para a edição existente
                ViewData["CurrentBudgetContext"] = "QualidadeDoAr"; // <<< Adicionado aqui
                return RedirectToAction("EditQualidadeDoAr", new { id = proposta.QualidadeDoArId });
            }

            if (treatmentType.Equals("QualidadeAr", StringComparison.OrdinalIgnoreCase))
            {
                var novoTratamentoAr = new QualidadeDoAr();
                _context.QualidadeDoAr.Add(novoTratamentoAr);
                await _context.SaveChangesAsync();

                proposta.QualidadeDoArId = novoTratamentoAr.Id;
                await _context.SaveChangesAsync();

                // Redireciona para a edição do novo tratamento
                ViewData["CurrentBudgetContext"] = "QualidadeDoAr"; // <<< Adicionado aqui
                return RedirectToAction("EditQualidadeDoAr", new { id = novoTratamentoAr.Id });
            }
            else
            {
                TempData["MensagemErro"] = "Tipo de tratamento desconhecido.";
                return RedirectToAction(nameof(SelectTreatment));
            }
        }

        //================================================================================
        // PÁGINAS DE EDIÇÃO (PLACEHOLDERS)
        // Adiciona ViewData["CurrentBudgetContext"] a todas elas
        //================================================================================
        [HttpGet]
        public IActionResult EditQualidadeDoAr(ulong id)
        {
            ViewData["Title"] = $"Editar Orçamento de Qualidade do Ar (ID: {id})";
            ViewData["CurrentBudgetContext"] = "QualidadeDoAr"; // <<< Adicionado aqui
            // TODO: Lógica para carregar os dados do tratamento com o ID recebido
            return View(); // Passa o ViewModel para a View
        }

        // TODO: Ações para Objetivos, Volumes, DetalheOrcamento, ResumoOrcamento.
        // Lembra-te de adicionar ViewData["CurrentBudgetContext"] = "QualidadeDoAr"; a cada uma delas.

        [HttpGet]
        public IActionResult ColecaoDados()
        {
            ViewData["Title"] = "Coleção de Dados";
            ViewData["CurrentBudgetContext"] = "QualidadeDoAr"; // <<< Adicionado aqui
            return View();
        }

        [HttpGet]
        public IActionResult Objetivos()
        {
            ViewData["Title"] = "Objetivos";
            ViewData["CurrentBudgetContext"] = "QualidadeDoAr"; // <<< Adicionado aqui
            return View();
        }

        [HttpGet]
        public IActionResult Volumes()
        {
            ViewData["Title"] = "Volumes";
            ViewData["CurrentBudgetContext"] = "QualidadeDoAr"; // <<< Adicionado aqui
            return View();
        }

        [HttpGet]
        public IActionResult DetalheOrcamento()
        {
            ViewData["Title"] = "Detalhe do Orçamento";
            ViewData["CurrentBudgetContext"] = "QualidadeDoAr"; // <<< Adicionado aqui
            return View();
        }

        [HttpGet]
        public IActionResult ResumoOrcamento()
        {
            ViewData["Title"] = "Resumo do Orçamento";
            ViewData["CurrentBudgetContext"] = "QualidadeDoAr"; // <<< Adicionado aqui
            return View();
        }

        // AÇÃO PARA APAGAR PROPOSTAS EM CURSO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProposta(ulong id)
        {
            var proposta = await _context.Proposta.FindAsync(id);

            if (proposta == null)
            {
                TempData["MensagemErro"] = "A proposta que tentou apagar não foi encontrada.";
                return RedirectToAction(nameof(OrçamentosEmCurso));
            }

            if (proposta.EstadoPropostaId != ESTADO_EM_CURSO)
            {
                TempData["MensagemErro"] = "Apenas propostas 'Em Curso' podem ser apagadas.";
                return RedirectToAction(nameof(OrçamentosEmCurso));
            }

            try
            {
                _context.Proposta.Remove(proposta);
                await _context.SaveChangesAsync();
                TempData["MensagemSucesso"] = $"A proposta Nº {id} foi apagada com sucesso.";
            }
            catch (DbUpdateException ex)
            {
                TempData["MensagemErro"] = $"Ocorreu um erro ao apagar a proposta: {ex.Message}";
            }

            return RedirectToAction(nameof(OrçamentosEmCurso));
        }
    }
}