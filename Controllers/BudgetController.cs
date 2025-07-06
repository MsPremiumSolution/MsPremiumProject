// File: MSPremiumProject/Controllers/BudgetController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Models;
using MSPremiumProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        // ================================================================================
        // Helper para definir o estado do submenu e a conclusão dos passos
        // ================================================================================
        private async Task SetQualidadeArSubmenuState(ulong propostaId, string activeLinkName)
        {
            ViewData["CurrentBudgetContext"] = "QualidadeAr";
            ViewData["ActiveSubmenuLink"] = activeLinkName;

            var stepsCompleted = new Dictionary<string, bool>();

            var proposta = await _context.Proposta
                                 .Include(p => p.QualidadeDoAr)
                                     .ThenInclude(qa => qa.DadosGerais)
                                         .ThenInclude(dg => dg.DadosConstrutivo)
                                 .Include(p => p.QualidadeDoAr)
                                     .ThenInclude(qa => qa.Objetivos)
                                 .Include(p => p.QualidadeDoAr)
                                     .ThenInclude(qa => qa.Volumes)
                                 .Include(p => p.Cliente)
                                 .FirstOrDefaultAsync(p => p.PropostaId == propostaId);

            if (proposta?.QualidadeDoAr != null)
            {
                ViewData["QualidadeDoArId"] = proposta.QualidadeDoAr.Id;
                ViewData["NomeCliente"] = $"{proposta.Cliente?.Nome} {proposta.Cliente?.Apelido}";

                stepsCompleted["ColecaoDados"] = proposta.QualidadeDoAr.DadosGeraisId.HasValue &&
                                                 proposta.QualidadeDoAr.DadosGerais?.DadosConstrutivo?.Id != 0 &&
                                                 proposta.QualidadeDoAr.DadosGerais?.Higrometria?.Id != 0 &&
                                                 proposta.QualidadeDoAr.DadosGerais?.Sintomatologia?.Id != 0;

                // Lógica de conclusão de Objetivos: considerada completa se pelo menos um tratamento for selecionado
                stepsCompleted["Objetivos"] = false; // Valor padrão
                if (proposta.QualidadeDoAr.Objetivos != null)
                {
                    stepsCompleted["Objetivos"] = proposta.QualidadeDoAr.Objetivos.IsolamentoExternoSATE ||
                                                  proposta.QualidadeDoAr.Objetivos.IsolamentoInteriorPladur ||
                                                  proposta.QualidadeDoAr.Objetivos.InjeccaoCamaraArPoliuretano ||
                                                  proposta.QualidadeDoAr.Objetivos.TrituracaoCorticaTriturada ||
                                                  proposta.QualidadeDoAr.Objetivos.AplicacaoTintaTermica ||
                                                  proposta.QualidadeDoAr.Objetivos.ImpermeabilizacaoFachadas ||
                                                  proposta.QualidadeDoAr.Objetivos.TubagemParedesInfiltracao ||
                                                  proposta.QualidadeDoAr.Objetivos.InjeccaoParedesAccaoCapilar ||
                                                  proposta.QualidadeDoAr.Objetivos.EvacuacaoHumidadeExcesso;
                }

                stepsCompleted["Volumes"] = proposta.QualidadeDoAr.Volumes != null && proposta.QualidadeDoAr.Volumes.Any();
                // Ou stepsCompleted["Volumes"] = proposta.QualidadeDoAr.VolumesId.HasValue; se for uma entidade única

                stepsCompleted["DetalheOrcamento"] = false;
                stepsCompleted["ResumoOrcamento"] = false;
            }
            else
            {
                ViewData["QualidadeDoArId"] = (ulong)0;
                ViewData["NomeCliente"] = "Cliente Não Encontrado";
                stepsCompleted["ColecaoDados"] = false;
                stepsCompleted["Objetivos"] = false;
                stepsCompleted["Volumes"] = false;
                stepsCompleted["DetalheOrcamento"] = false;
                stepsCompleted["ResumoOrcamento"] = false;
            }

            ViewData["StepsCompleted"] = stepsCompleted;
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
            TempData["Debug: IniciarOrcamento - Start"] = "Iniciando a acao IniciarOrcamento.";
            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null)
            {
                TempData["MensagemErro"] = $"Cliente ID {clienteId} não encontrado.";
                TempData["Debug: IniciarOrcamento - Client NotFound"] = "Cliente não encontrado, redirecionando para Index.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Debug: IniciarOrcamento - Client Found"] = $"Cliente '{cliente.Nome} {cliente.Apelido}' encontrado.";

            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    TempData["MensagemErro"] = "Utilizador não autenticado.";
                    return RedirectToAction("Login", "Account");
                }

                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    TempData["MensagemErro"] = "ID de utilizador não encontrado no token de autenticação.";
                    return RedirectToAction("Login", "Account");
                }
                var utilizadorId = ulong.Parse(userIdClaim);
                TempData["Debug: IniciarOrcamento - UserId"] = $"User ID {utilizadorId} obtido com sucesso.";

                var propostasIncompletas = await _context.Proposta
                    .Where(p => p.ClienteId == clienteId &&
                                p.UtilizadorId == utilizadorId &&
                                p.EstadoPropostaId == ESTADO_EM_CURSO &&
                                p.QualidadeDoArId == null)
                    .OrderByDescending(p => p.DataProposta)
                    .ToListAsync();

                Proposta propostaParaTrabalhar;

                if (propostasIncompletas.Any())
                {
                    propostaParaTrabalhar = propostasIncompletas.First();

                    var propostasParaRemover = propostasIncompletas.Skip(1);
                    if (propostasParaRemover.Any())
                    {
                        _context.Proposta.RemoveRange(propostasParaRemover);
                        await _context.SaveChangesAsync();
                        TempData["Debug: IniciarOrcamento - Cleaned Orphans"] = $"{propostasParaRemover.Count()} propostas órfãs foram limpas.";
                    }
                    TempData["Debug: IniciarOrcamento - Found Existing Proposal"] = $"Continuando proposta existente ID: {propostaParaTrabalhar.PropostaId}";
                }
                else
                {
                    propostaParaTrabalhar = new Proposta
                    {
                        ClienteId = clienteId,
                        UtilizadorId = utilizadorId,
                        EstadoPropostaId = ESTADO_EM_CURSO,
                        DataProposta = DateTime.UtcNow
                    };
                    _context.Proposta.Add(propostaParaTrabalhar);
                    await _context.SaveChangesAsync();
                    TempData["Debug: IniciarOrcamento - New Proposal Created"] = $"Nova proposta criada com ID: {propostaParaTrabalhar.PropostaId}";
                }

                HttpContext.Session.SetString("CurrentPropostaId", propostaParaTrabalhar.PropostaId.ToString());
                TempData["Debug: IniciarOrcamento - Session Set"] = $"Proposta ID {propostaParaTrabalhar.PropostaId} salva na sessão.";

                return RedirectToAction(nameof(TipologiaConstrutiva));
            }
            catch (FormatException fEx)
            {
                TempData["MensagemErro"] = $"Erro de formato ao obter ID do utilizador: {fEx.Message}.";
                TempData["Debug: IniciarOrcamento - FormatError"] = $"FormatException: {fEx.Message}";
                return RedirectToAction("Login", "Account");
            }
            catch (DbUpdateException dbEx)
            {
                TempData["MensagemErro"] = $"Erro DB ao iniciar orçamento: Falha ao guardar dados na base de dados. Detalhes: {dbEx.Message}";
                TempData["Debug: IniciarOrcamento - DbUpdateError"] = $"DbUpdateException: {dbEx.InnerException?.Message ?? dbEx.Message}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro inesperado ao iniciar orçamento: {ex.Message}.";
                TempData["Debug: IniciarOrcamento - GeneralError"] = $"GeneralException: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        //================================================================================
        // ETAPA 2.5: CONTINUAR ORÇAMENTO (AJUSTADO PARA REDIRECIONAR PARA O PRIMEIRO PASSO INCOMPLETO)
        //================================================================================
        [HttpGet]
        public async Task<IActionResult> ContinuarOrcamento(ulong id) // id aqui é o PropostaId
        {
            // Carrega a proposta e todas as sub-entidades necessárias para a verificação de progresso
            var proposta = await _context.Proposta
                                     .Include(p => p.QualidadeDoAr)
                                         .ThenInclude(qa => qa.DadosGerais)
                                             .ThenInclude(dg => dg.DadosConstrutivo) // Para verificar a coleção de dados
                                     .Include(p => p.QualidadeDoAr)
                                         .ThenInclude(qa => qa.Objetivos) // Para verificar os objetivos
                                     .Include(p => p.QualidadeDoAr)
                                         .ThenInclude(qa => qa.Volumes) // Para verificar os volumes
                                     .FirstOrDefaultAsync(p => p.PropostaId == id);

            if (proposta == null || proposta.EstadoPropostaId != ESTADO_EM_CURSO)
            {
                TempData["MensagemErro"] = "Orçamento inválido ou já concluído.";
                return RedirectToAction(nameof(OrçamentosEmCurso));
            }

            HttpContext.Session.SetString("CurrentPropostaId", proposta.PropostaId.ToString());

            // --- LÓGICA DE REDIRECIONAMENTO INTELIGENTE ---

            // Passo 0: Verificar se a Tipologia Construtiva foi escolhida
            if (proposta.TipologiaConstrutivaId == null)
            {
                return RedirectToAction(nameof(TipologiaConstrutiva));
            }

            // Passo 1: Verificar se a estrutura de Qualidade do Ar foi criada
            if (proposta.QualidadeDoArId == null)
            {
                return RedirectToAction(nameof(SelectTreatment));
            }

            // A partir daqui, a estrutura de Qualidade do Ar existe.
            var qaId = proposta.QualidadeDoArId.Value;
            var qualidadeDoAr = proposta.QualidadeDoAr;

            // Passo 2: Verificar a "Coleção de Dados"
            bool colecaoDadosCompleta = qualidadeDoAr.DadosGeraisId.HasValue &&
                                        qualidadeDoAr.DadosGerais?.DadosConstrutivo?.Id != 0 &&
                                        qualidadeDoAr.DadosGerais?.Higrometria?.Id != 0 &&
                                        qualidadeDoAr.DadosGerais?.Sintomatologia?.Id != 0;

            if (!colecaoDadosCompleta)
            {
                return RedirectToAction("EditQualidadeDoAr", new { id = qaId });
            }

            // Passo 3: Verificar "Objetivos"
            // (Considerado completo se pelo menos um tratamento for selecionado)
            bool objetivosCompletos = false;
            if (qualidadeDoAr.Objetivos != null)
            {
                objetivosCompletos = qualidadeDoAr.Objetivos.IsolamentoExternoSATE ||
                                     qualidadeDoAr.Objetivos.IsolamentoInteriorPladur ||
                                     qualidadeDoAr.Objetivos.InjeccaoCamaraArPoliuretano ||
                                     qualidadeDoAr.Objetivos.TrituracaoCorticaTriturada ||
                                     qualidadeDoAr.Objetivos.AplicacaoTintaTermica ||
                                     qualidadeDoAr.Objetivos.ImpermeabilizacaoFachadas ||
                                     qualidadeDoAr.Objetivos.TubagemParedesInfiltracao ||
                                     qualidadeDoAr.Objetivos.InjeccaoParedesAccaoCapilar ||
                                     qualidadeDoAr.Objetivos.EvacuacaoHumidadeExcesso;
            }

            if (!objetivosCompletos)
            {
                return RedirectToAction("Objetivos", new { id = qaId });
            }

            // Passo 4: Verificar "Volumes"
            // (Considerado completo se pelo menos um volume foi adicionado)
            bool volumesCompletos = qualidadeDoAr.Volumes != null && qualidadeDoAr.Volumes.Any();
            if (!volumesCompletos)
            {
                return RedirectToAction("Volumes", new { id = qaId });
            }

            // Passo 5: Verificar "Detalhe do Orçamento" (adicione a sua lógica de conclusão aqui)
            // Exemplo: bool detalheOrcamentoCompleto = (qualidadeDoAr.OrcamentoAr?.AlgumCampoImportante > 0);
            bool detalheOrcamentoCompleto = false; // Substitua pela sua lógica real.
            if (!detalheOrcamentoCompleto)
            {
                return RedirectToAction("DetalheOrcamento", new { id = qaId });
            }

            // Se todos os passos acima estiverem completos, redireciona para o resumo como o próximo passo.
            // Se o resumo for o último passo, o utilizador pode querer editá-lo.
            return RedirectToAction("ResumoOrcamento", new { id = qaId });
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

            if (proposta.QualidadeDoArId.HasValue)
            {
                return RedirectToAction("EditQualidadeDoAr", new { id = proposta.QualidadeDoArId.Value });
            }

            ViewData["ClienteNome"] = $"{proposta.Cliente.Nome} {proposta.Cliente.Apelido}";
            ViewData["SelectedTipologiaId"] = proposta.TipologiaConstrutivaId;

            try
            {
                var tipologiasDisponiveis = await _context.TipologiasConstrutivas.OrderBy(t => t.Nome).ToListAsync();
                return View(tipologiasDisponiveis);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao carregar Tipologias Construtivas: {ex.Message}. Verifique logs e seeding de TipologiasConstrutivas.";
                return RedirectToAction(nameof(OrçamentosEmCurso));
            }
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

            if (proposta.QualidadeDoArId.HasValue)
            {
                await SetQualidadeArSubmenuState(propostaId, "SelectTreatment");
            }

            var viewModel = new SelectTreatmentViewModel
            {
                NomeCliente = $"{proposta.Cliente.Nome} {proposta.Cliente.Apelido}"
            };

            return View(viewModel);
        }

        //================================================================================
        // ETAPA 5: PROCESSAR ESCOLHA E CRIAR TODA A ESTRUTURA DE DADOS
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
                return RedirectToAction(nameof(ContinuarOrcamento), new { id = proposta.PropostaId });
            }

            if (treatmentType.Equals("QualidadeAr", StringComparison.OrdinalIgnoreCase))
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var novosDadosConstrutivos = new DadosConstrutivos { DataVisita = DateTime.Today };
                    var novaHigrometria = new Higrometria();
                    var novaSintomatologia = new Sintomatologia();
                    var novosObjetivos = new Objetivos();
                    var novoOrcamentoAr = new OrcamentoAr();

                    _context.AddRange(novosDadosConstrutivos, novaHigrometria, novaSintomatologia, novosObjetivos, novoOrcamentoAr);
                    await _context.SaveChangesAsync();

                    var novosDadosGerais = new DadosGerais
                    {
                        DadosConstrutivosId = novosDadosConstrutivos.Id,
                        HigrometriaId = novaHigrometria.Id,
                        SintomalogiaId = novaSintomatologia.Id
                    };
                    _context.DadosGerais.Add(novosDadosGerais);
                    await _context.SaveChangesAsync();

                    var novoTratamentoAr = new QualidadeDoAr
                    {
                        PropostaId = proposta.PropostaId, // <<< ADICIONE ESTA LINHA AQUI
                        DadosGeraisId = novosDadosGerais.Id,
                        ObjetivosId = novosObjetivos.Id,
                        OrcamentoArId = novoOrcamentoAr.Id
                    };
                    _context.QualidadeDoAr.Add(novoTratamentoAr);
                    await _context.SaveChangesAsync();

                    proposta.QualidadeDoArId = novoTratamentoAr.Id;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return RedirectToAction("EditQualidadeDoAr", new { id = novoTratamentoAr.Id });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["MensagemErro"] = $"Erro crítico ao criar estrutura do orçamento: {ex.Message}. Tente novamente.";
                    return RedirectToAction(nameof(SelectTreatment));
                }
            }
            TempData["MensagemErro"] = "Tipo de tratamento desconhecido.";
            return RedirectToAction(nameof(SelectTreatment));
        }

        //================================================================================
        // PÁGINAS DE EDIÇÃO - TODAS ATIVAM O SUBMENU E O LINK CORRETO
        //================================================================================
        [HttpGet]
        public async Task<IActionResult> ColecaoDados()
        {
            if (!ulong.TryParse(HttpContext.Session.GetString("CurrentPropostaId"), out ulong propostaId))
            {
                TempData["MensagemErro"] = "Sessão expirada. Por favor, retome o orçamento.";
                return RedirectToAction(nameof(OrçamentosEmCurso));
            }

            var proposta = await _context.Proposta.FindAsync(propostaId);
            if (proposta == null || !proposta.QualidadeDoArId.HasValue)
            {
                TempData["MensagemErro"] = "Orçamento de Qualidade do Ar não encontrado ou não iniciado.";
                return RedirectToAction(nameof(SelectTreatment));
            }

            return RedirectToAction(nameof(EditQualidadeDoAr), new { id = proposta.QualidadeDoArId.Value });
        }

        [HttpGet]
        public async Task<IActionResult> EditQualidadeDoAr(ulong id)
        {
            ViewData["Title"] = $"Editar Orçamento de Qualidade do Ar (ID: {id})";

            if (!ulong.TryParse(HttpContext.Session.GetString("CurrentPropostaId"), out ulong propostaId))
            {
                TempData["MensagemErro"] = "Sessão expirada.";
                return RedirectToAction(nameof(OrçamentosEmCurso));
            }
            await SetQualidadeArSubmenuState(propostaId, "ColecaoDados");

            var proposta = await _context.Proposta.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.PropostaId == propostaId);
            if (proposta == null)
            {
                TempData["MensagemErro"] = "Proposta associada ao orçamento de Qualidade do Ar não encontrada.";
                return NotFound();
            }

            var tratamento = await _context.QualidadeDoAr
                .Include(q => q.DadosGerais)
                    .ThenInclude(dg => dg.DadosConstrutivo)
                        .ThenInclude(dc => dc.Janelas)
                .Include(q => q.DadosGerais)
                    .ThenInclude(dg => dg.Higrometria)
                .Include(q => q.DadosGerais)
                    .ThenInclude(dg => dg.Sintomatologia)
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == id);

            if (tratamento == null || tratamento.DadosGerais == null || tratamento.DadosGerais.DadosConstrutivo == null || tratamento.DadosGerais.Higrometria == null || tratamento.DadosGerais.Sintomatologia == null)
            {
                TempData["MensagemErro"] = "A estrutura de dados para este orçamento não foi encontrada ou está incompleta.";
                return NotFound();
            }

            var primeiraJanela = tratamento.DadosGerais.DadosConstrutivo.Janelas?.FirstOrDefault();

            var tiposJanelaDisponiveis = await _context.Tipojanelas
                .OrderBy(tj => tj.TipoJanela1)
                .Select(tj => new SelectListItem
                {
                    Value = tj.TipoJanela1,
                    Text = tj.TipoJanela1
                })
                .ToListAsync();

            var viewModel = new QualidadeArViewModel
            {
                PropostaId = propostaId,
                QualidadeDoArId = tratamento.Id,
                NomeCliente = $"{proposta.Cliente.Nome} {proposta.Cliente.Apelido}",

                DataVisita = tratamento.DadosGerais.DadosConstrutivo.DataVisita,
                AnoConstrucao = tratamento.DadosGerais.DadosConstrutivo.AnoConstrucao,
                AreaM2 = tratamento.DadosGerais.DadosConstrutivo.AreaM2,
                NumeroAndares = tratamento.DadosGerais.DadosConstrutivo.NumeroAndares,
                NumeroHabitantes = tratamento.DadosGerais.DadosConstrutivo.NumeroHabitantes,
                Localidade = tratamento.DadosGerais.DadosConstrutivo.Localidade,
                Altitude = tratamento.DadosGerais.DadosConstrutivo.Altitude,
                TipoFachada = tratamento.DadosGerais.DadosConstrutivo.TipoFachada,
                OrientacaoFachada = tratamento.DadosGerais.DadosConstrutivo.OrientacaoFachada,
                CoberturaFachadaPrincipal = tratamento.DadosGerais.DadosConstrutivo.CoberturaFachadaPrincipal,
                CoberturaFachadaPosterior = tratamento.DadosGerais.DadosConstrutivo.CoberturaFachadaPosterior,
                TratamentoHidrofugacao = tratamento.DadosGerais.DadosConstrutivo.TratamentoHidrofugacao,
                IsolamentoCamara = tratamento.DadosGerais.DadosConstrutivo.IsolamentoCamara,
                IsolamentoInterno = tratamento.DadosGerais.DadosConstrutivo.IsolamentoInterno,
                TipoAquecimento = tratamento.DadosGerais.DadosConstrutivo.TipoAquecimento,

                JanelaId = primeiraJanela?.Id,
                TipoJanelaPrincipal = primeiraJanela?.TipoJanela,
                TiposJanelaDisponiveis = tiposJanelaDisponiveis,

                MaterialJanela = primeiraJanela?.Material,
                JanelasDuplas = primeiraJanela?.PossuiJanelasDuplas,
                TipoVidro = primeiraJanela?.TipoVidro,
                RPT = primeiraJanela?.PossuiRPT,
                CaixasPersiana = primeiraJanela?.PossuiCaixaPersiana,
                NumeroUnidadesJanela = primeiraJanela?.NumeroUnidades,

                HumidadeRelativaExterior = tratamento.DadosGerais.Higrometria.HumidadeRelativaExterior,
                TemperaturaExterior = tratamento.DadosGerais.Higrometria.TemperaturaExterior,
                HumidadeRelativaInterior = tratamento.DadosGerais.Higrometria.HumidadeRelativaInterior,
                TemperaturaInterior = tratamento.DadosGerais.Higrometria.TemperaturaInterior,
                TemperaturaParedesInternas = tratamento.DadosGerais.Higrometria.TemperaturaParedesInternas,

                PontoDeOrvalho = tratamento.DadosGerais.Higrometria.PontoDeOrvalho,
                PontosFrios = tratamento.DadosGerais.Higrometria.PontosFrios,

                NivelCO2 = tratamento.DadosGerais.Higrometria.NivelCO2,
                NivelTCOV = tratamento.DadosGerais.Higrometria.NivelTCOV,
                NivelHCHO = tratamento.DadosGerais.Higrometria.NivelHCHO,
                DataLoggerSensores = tratamento.DadosGerais.Higrometria.DataLoggerSensores,

                Fungos = tratamento.DadosGerais.Sintomatologia.Fungos,
                Cheiros = tratamento.DadosGerais.Sintomatologia.Cheiros,
                MofoEmRoupasArmarios = tratamento.DadosGerais.Sintomatologia.MofoEmRoupasArmarios,
                CondensacaoNasJanelas = tratamento.DadosGerais.Sintomatologia.CondensacaoNasJanelas,
                ConsumoExcessivoAquecimento = tratamento.DadosGerais.Sintomatologia.ConsumoExcessivoAquecimento,
                Alergias = tratamento.DadosGerais.Sintomatologia.Alergias,
                ProblemasRespiratorios = tratamento.DadosGerais.Sintomatologia.ProblemasRespiratorios,
                GasRadao = tratamento.DadosGerais.Sintomatologia.GasRadao,
                EsporosEmSuperficies = tratamento.DadosGerais.Sintomatologia.EsporosEmSuperficies
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQualidadeDoAr(QualidadeArViewModel model)
        {
            await SetQualidadeArSubmenuState(model.PropostaId, "ColecaoDados");

            if (!ModelState.IsValid)
            {
                var propostaCliente = await _context.Proposta.Include(p => p.Cliente).AsNoTracking().FirstOrDefaultAsync(p => p.PropostaId == model.PropostaId);
                if (propostaCliente != null)
                {
                    model.NomeCliente = $"{propostaCliente.Cliente.Nome} {propostaCliente.Cliente.Apelido}";
                }

                model.TiposJanelaDisponiveis = await _context.Tipojanelas
                    .OrderBy(tj => tj.TipoJanela1)
                    .Select(tj => new SelectListItem
                    {
                        Value = tj.TipoJanela1,
                        Text = tj.TipoJanela1
                    })
                    .ToListAsync();

                return View(model);
            }

            var tratamentoParaAtualizar = await _context.QualidadeDoAr
                .Include(q => q.DadosGerais)
                    .ThenInclude(dg => dg.DadosConstrutivo)
                        .ThenInclude(dc => dc.Janelas)
                .Include(q => q.DadosGerais)
                    .ThenInclude(dg => dg.Higrometria)
                .Include(q => q.DadosGerais)
                    .ThenInclude(dg => dg.Sintomatologia)
                .FirstOrDefaultAsync(q => q.Id == model.QualidadeDoArId);

            if (tratamentoParaAtualizar == null || tratamentoParaAtualizar.DadosGerais == null ||
                tratamentoParaAtualizar.DadosGerais.DadosConstrutivo == null ||
                tratamentoParaAtualizar.DadosGerais.Higrometria == null ||
                tratamentoParaAtualizar.DadosGerais.Sintomatologia == null)
            {
                TempData["MensagemErro"] = "Dados do orçamento incompletos ou não encontrados para atualização.";
                return NotFound();
            }

            var dc = tratamentoParaAtualizar.DadosGerais.DadosConstrutivo;
            dc.DataVisita = model.DataVisita;
            dc.AnoConstrucao = model.AnoConstrucao;
            dc.AreaM2 = model.AreaM2;
            dc.NumeroAndares = model.NumeroAndares;
            dc.NumeroHabitantes = model.NumeroHabitantes;
            dc.Localidade = model.Localidade;
            dc.Altitude = model.Altitude;
            dc.TipoFachada = model.TipoFachada;
            dc.OrientacaoFachada = model.OrientacaoFachada;
            dc.CoberturaFachadaPrincipal = model.CoberturaFachadaPrincipal;
            dc.CoberturaFachadaPosterior = model.CoberturaFachadaPosterior;
            dc.TratamentoHidrofugacao = model.TratamentoHidrofugacao ?? false;

            dc.IsolamentoCamara = model.IsolamentoCamara;
            dc.IsolamentoInterno = model.IsolamentoInterno;
            dc.TipoAquecimento = model.TipoAquecimento;

            Janela? janelaExistente = dc.Janelas.FirstOrDefault();

            bool anyWindowDataProvided = !string.IsNullOrEmpty(model.TipoJanelaPrincipal) ||
                                         !string.IsNullOrEmpty(model.MaterialJanela) ||
                                         !string.IsNullOrEmpty(model.TipoVidro) ||
                                         model.NumeroUnidadesJanela.HasValue ||
                                         model.JanelasDuplas.HasValue ||
                                         model.RPT.HasValue ||
                                         model.CaixasPersiana.HasValue;

            if (anyWindowDataProvided)
            {
                if (janelaExistente == null)
                {
                    janelaExistente = new Janela { DadosConstrutivosId = dc.Id };
                    _context.Janelas.Add(janelaExistente);
                }
                janelaExistente.TipoJanela = model.TipoJanelaPrincipal;
                janelaExistente.Material = model.MaterialJanela;
                janelaExistente.TipoVidro = model.TipoVidro;
                janelaExistente.NumeroUnidades = model.NumeroUnidadesJanela;
                janelaExistente.PossuiJanelasDuplas = model.JanelasDuplas;
                janelaExistente.PossuiRPT = model.RPT;
                janelaExistente.PossuiCaixaPersiana = model.CaixasPersiana;
            }
            else
            {
                if (janelaExistente != null)
                {
                    _context.Janelas.Remove(janelaExistente);
                }
            }

            var hg = tratamentoParaAtualizar.DadosGerais.Higrometria;
            hg.HumidadeRelativaExterior = model.HumidadeRelativaExterior;
            hg.TemperaturaExterior = model.TemperaturaExterior;
            hg.HumidadeRelativaInterior = model.HumidadeRelativaInterior;
            hg.TemperaturaInterior = model.TemperaturaInterior;
            hg.TemperaturaParedesInternas = model.TemperaturaParedesInternas;

            hg.PontoDeOrvalho = model.PontoDeOrvalho;
            hg.PontosFrios = model.PontosFrios;

            hg.NivelCO2 = model.NivelCO2;
            hg.NivelTCOV = model.NivelTCOV;
            hg.NivelHCHO = model.NivelHCHO;
            hg.DataLoggerSensores = model.DataLoggerSensores;

            var st = tratamentoParaAtualizar.DadosGerais.Sintomatologia;
            st.Fungos = model.Fungos ?? false;
            st.Cheiros = model.Cheiros ?? false;
            st.MofoEmRoupasArmarios = model.MofoEmRoupasArmarios ?? false;
            st.CondensacaoNasJanelas = model.CondensacaoNasJanelas ?? false;
            st.ConsumoExcessivoAquecimento = model.ConsumoExcessivoAquecimento ?? false;
            st.Alergias = model.Alergias ?? false;
            st.ProblemasRespiratorios = model.ProblemasRespiratorios ?? false;
            st.GasRadao = model.GasRadao ?? false;
            st.EsporosEmSuperficies = model.EsporosEmSuperficies ?? false;

            await _context.SaveChangesAsync();
            TempData["MensagemSucesso"] = "Dados de Qualidade do Ar guardados com sucesso!";
            return RedirectToAction("Objetivos", new { id = model.QualidadeDoArId });
        }

        // Objetivos (GET) - Exibe apenas 'Possíveis tratamentos'
        [HttpGet]
        public async Task<IActionResult> Objetivos(ulong id)
        {
            if (!ulong.TryParse(HttpContext.Session.GetString("CurrentPropostaId"), out ulong propostaId))
            {
                TempData["MensagemErro"] = "Sessão expirada. Por favor, retome o orçamento.";
                return RedirectToAction(nameof(OrçamentosEmCurso));
            }

            await SetQualidadeArSubmenuState(propostaId, "Objetivos");

            // A busca da proposta ainda é útil para definir o título da página.
            var proposta = await _context.Proposta.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.PropostaId == propostaId);
            if (proposta == null)
            {
                TempData["MensagemErro"] = "Proposta associada ao orçamento de Qualidade do Ar não encontrada.";
                return NotFound();
            }

            var qualidadeDoAr = await _context.QualidadeDoAr
                .Include(qa => qa.Objetivos)
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == id);

            if (qualidadeDoAr == null || qualidadeDoAr.Objetivos == null)
            {
                TempData["MensagemErro"] = "A estrutura de objetivos para este orçamento não foi encontrada ou está incompleta. Por favor, reinicie a criação do orçamento ou contacte o suporte.";
                return RedirectToAction(nameof(SelectTreatment));
            }

            // O título pode ser definido diretamente no ViewData, sem precisar do ViewModel para isso.
            ViewData["Title"] = $"Objetivos - {proposta.Cliente.Nome} {proposta.Cliente.Apelido}";

            var viewModel = new ObjetivosViewModel
            {
                PropostaId = propostaId,
                QualidadeDoArId = qualidadeDoAr.Id,
                // REMOVIDO: NomeCliente = $"{proposta.Cliente.Nome} {proposta.Cliente.Apelido}",

                IsolamentoExternoSATE = qualidadeDoAr.Objetivos.IsolamentoExternoSATE,
                IsolamentoInteriorPladur = qualidadeDoAr.Objetivos.IsolamentoInteriorPladur,
                InjeccaoCamaraArPoliuretano = qualidadeDoAr.Objetivos.InjeccaoCamaraArPoliuretano,
                TrituracaoCorticaTriturada = qualidadeDoAr.Objetivos.TrituracaoCorticaTriturada,
                AplicacaoTintaTermica = qualidadeDoAr.Objetivos.AplicacaoTintaTermica,
                ImpermeabilizacaoFachadas = qualidadeDoAr.Objetivos.ImpermeabilizacaoFachadas,
                TubagemParedesInfiltracao = qualidadeDoAr.Objetivos.TubagemParedesInfiltracao,
                InjeccaoParedesAccaoCapilar = qualidadeDoAr.Objetivos.InjeccaoParedesAccaoCapilar,
                EvacuacaoHumidadeExcesso = qualidadeDoAr.Objetivos.EvacuacaoHumidadeExcesso
            };

            return View("BudgetGoals", viewModel);
        }

        // Objetivos (POST) - Salva apenas 'Possíveis tratamentos'
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Objetivos(ObjetivosViewModel model)
        {
            await SetQualidadeArSubmenuState(model.PropostaId, "Objetivos");

            if (!ModelState.IsValid)
            {
                // Se o ModelState não for válido, defina o título da página antes de retornar a view.
                var propostaCliente = await _context.Proposta.Include(p => p.Cliente).AsNoTracking().FirstOrDefaultAsync(p => p.PropostaId == model.PropostaId);
                if (propostaCliente != null)
                {
                    ViewData["Title"] = $"Objetivos - {propostaCliente.Cliente.Nome} {propostaCliente.Cliente.Apelido}";
                }
                return View("BudgetGoals", model);
            }

            var qualidadeDoArParaAtualizar = await _context.QualidadeDoAr
                .Include(qa => qa.Objetivos)
                .FirstOrDefaultAsync(q => q.Id == model.QualidadeDoArId);

            if (qualidadeDoArParaAtualizar == null || qualidadeDoArParaAtualizar.Objetivos == null)
            {
                TempData["MensagemErro"] = "Dados de objetivos incompletos ou não encontrados para atualização.";
                return NotFound();
            }

            var objetivosToUpdate = qualidadeDoArParaAtualizar.Objetivos;

            objetivosToUpdate.IsolamentoExternoSATE = model.IsolamentoExternoSATE;
            objetivosToUpdate.IsolamentoInteriorPladur = model.IsolamentoInteriorPladur;
            objetivosToUpdate.InjeccaoCamaraArPoliuretano = model.InjeccaoCamaraArPoliuretano;
            objetivosToUpdate.TrituracaoCorticaTriturada = model.TrituracaoCorticaTriturada;
            objetivosToUpdate.AplicacaoTintaTermica = model.AplicacaoTintaTermica;
            objetivosToUpdate.ImpermeabilizacaoFachadas = model.ImpermeabilizacaoFachadas;
            objetivosToUpdate.TubagemParedesInfiltracao = model.TubagemParedesInfiltracao;
            objetivosToUpdate.InjeccaoParedesAccaoCapilar = model.InjeccaoParedesAccaoCapilar;
            objetivosToUpdate.EvacuacaoHumidadeExcesso = model.EvacuacaoHumidadeExcesso;

            await _context.SaveChangesAsync();
            TempData["MensagemSucesso"] = "Objetivos guardados com sucesso!";

            return RedirectToAction("Volumes", new { id = model.QualidadeDoArId });
        }

        //================================================================================
        // PÁGINAS PLACEHOLDER PARA AS OUTRAS SUB-ETAPAS DA QUALIDADE DO AR
        // Todas ativam o submenu e o link correto e aceitam 'id'
        //================================================================================
        [HttpGet]
        public async Task<IActionResult> Volumes(ulong id) // id é o QualidadeDoArId
        {
            if (!ulong.TryParse(HttpContext.Session.GetString("CurrentPropostaId"), out ulong propostaId))
            {
                TempData["MensagemErro"] = "Sessão expirada. Por favor, retome o orçamento.";
                return RedirectToAction(nameof(OrçamentosEmCurso));
            }

            await SetQualidadeArSubmenuState(propostaId, "Volumes");

            var proposta = await _context.Proposta.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.PropostaId == propostaId);
            if (proposta == null || !proposta.QualidadeDoArId.HasValue)
            {
                TempData["MensagemErro"] = "Orçamento de Qualidade do Ar não encontrado ou não iniciado.";
                return RedirectToAction(nameof(SelectTreatment));
            }

            // Carregar os volumes e medidas existentes
            var volumesDb = await _context.Volumes
                .Include(v => v.Medidas)
                .Where(v => v.QualidadeDoArId == id)
                .ToListAsync();

            var viewModel = new VolumesViewModel
            {
                PropostaId = propostaId,
                QualidadeDoArId = id,
                //NomeCliente = $"{proposta.Cliente?.Nome} {proposta.Cliente?.Apelido}",
                Volumes = volumesDb.Select(v => new VolumeItemViewModel
                {
                    Id = v.Id,
                    Altura = v.Altura,
                    Medidas = v.Medidas.Select(m => new MedidaItemViewModel
                    {
                        Id = m.Id,
                        Largura = m.Largura,
                        Comprimento = m.Comprimento
                    }).ToList()
                }).ToList()
            };

            // Se não houver volumes, adiciona um em branco para começar
            if (!viewModel.Volumes.Any())
            {
                viewModel.Volumes.Add(new VolumeItemViewModel());
            }

            ViewData["Title"] = "Volumes";
            return View(viewModel);
        }

        // Volumes (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Volumes(VolumesViewModel model)
        {
            await SetQualidadeArSubmenuState(model.PropostaId, "Volumes");

            if (!ModelState.IsValid)
            {
                // Se o modelo não for válido, recalcular totais para exibição correta na view
                // (O JavaScript já faz isso no lado do cliente, mas é bom ter aqui como fallback)
                return View(model);
            }

            // Precisamos da entidade QualidadeDoAr para salvar os totais.
            var qualidadeDoAr = await _context.QualidadeDoAr.FindAsync(model.QualidadeDoArId);
            if (qualidadeDoAr == null)
            {
                TempData["MensagemErro"] = "Orçamento de Qualidade do Ar não encontrado para atualização.";
                return NotFound();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Remover todos os volumes e medidas existentes para este orçamento
                var existingVolumes = await _context.Volumes
                    .Include(v => v.Medidas)
                    .Where(v => v.QualidadeDoArId == model.QualidadeDoArId)
                    .ToListAsync();

                foreach (var volume in existingVolumes)
                {
                    _context.Medida.RemoveRange(volume.Medidas);
                }
                _context.Volumes.RemoveRange(existingVolumes);
                await _context.SaveChangesAsync(); // Aplica as remoções

                // 2. Calcular totais e adicionar os novos volumes e medidas
                decimal volumeTotalCalculado = 0;
                decimal superficieTotalCalculada = 0;

                if (model.Volumes != null)
                {
                    foreach (var volumeVm in model.Volumes)
                    {
                        if (volumeVm.Altura <= 0) continue;

                        var newVolume = new Volume
                        {
                            QualidadeDoArId = model.QualidadeDoArId,
                            Altura = volumeVm.Altura
                        };

                        _context.Volumes.Add(newVolume);
                        // É importante salvar aqui para que o newVolume.Id seja gerado para as medidas
                        await _context.SaveChangesAsync();

                        decimal superficieDoVolume = 0;
                        if (volumeVm.Medidas != null)
                        {
                            foreach (var medidaVm in volumeVm.Medidas)
                            {
                                if (medidaVm.Largura > 0 && medidaVm.Comprimento > 0)
                                {
                                    var newMedida = new Medida
                                    {
                                        VolumeId = newVolume.Id,
                                        Largura = medidaVm.Largura,
                                        Comprimento = medidaVm.Comprimento
                                    };
                                    _context.Medida.Add(newMedida);
                                    superficieDoVolume += medidaVm.Largura * medidaVm.Comprimento;
                                }
                            }
                        }

                        // Acumular os totais
                        superficieTotalCalculada += superficieDoVolume;
                        volumeTotalCalculado += superficieDoVolume * newVolume.Altura;
                    }
                }

                // 3. Atribuir os totais calculados à entidade QualidadeDoAr
                qualidadeDoAr.SuperficieTotal = superficieTotalCalculada;
                qualidadeDoAr.VolumeTotal = volumeTotalCalculado;

                // Marca a entidade como modificada. O EF Core fará o UPDATE.
                _context.QualidadeDoAr.Update(qualidadeDoAr);

                await _context.SaveChangesAsync(); // Salva as novas medidas e os totais na QualidadeDoAr
                await transaction.CommitAsync();

                TempData["MensagemSucesso"] = "Volumes guardados com sucesso!";
                return RedirectToAction("DetalheOrcamento", new { id = model.QualidadeDoArId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["MensagemErro"] = $"Erro ao guardar os volumes: {ex.Message}";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetalheOrcamento(ulong id) // id é o QualidadeDoArId
        {
            if (!ulong.TryParse(HttpContext.Session.GetString("CurrentPropostaId"), out ulong propostaId))
            {
                TempData["MensagemErro"] = "Sessão expirada. Por favor, retome o orçamento.";
                return RedirectToAction(nameof(OrçamentosEmCurso));
            }
            await SetQualidadeArSubmenuState(propostaId, "DetalheOrcamento");

            var proposta = await _context.Proposta.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.PropostaId == propostaId);
            if (proposta == null || !proposta.QualidadeDoArId.HasValue)
            {
                TempData["MensagemErro"] = "Orçamento de Qualidade do Ar não encontrado ou não iniciado.";
                return RedirectToAction(nameof(SelectTreatment));
            }

            var orcamentoAr = await _context.OrcamentoAr
                .Include(oa => oa.LinhasOrcamento) // Carrega as linhas existentes
                .FirstOrDefaultAsync(oa => oa.QualidadeDoAr.Id == id);

            var qualidadeDoAr = await _context.QualidadeDoAr.FindAsync(id);

            if (orcamentoAr == null || qualidadeDoAr == null)
            {
                TempData["MensagemErro"] = "Estrutura do orçamento não encontrada.";
                return RedirectToAction(nameof(SelectTreatment));
            }

            // Função auxiliar para obter uma linha do orçamento
            Func<string, OrcamentoArLinha> getLinha = (codigo) =>
                orcamentoAr.LinhasOrcamento.FirstOrDefault(l => l.CodigoItem == codigo);

            var viewModel = new DetalheOrcamentoViewModel
            {
                PropostaId = propostaId,
                QualidadeDoArId = id,
                OrcamentoArId = orcamentoAr.Id,
                //NomeCliente = $"{proposta.Cliente?.Nome} {proposta.Cliente?.Apelido}",

                // Carregar dados salvos
                M3VolumeHabitavel = orcamentoAr.M3VolumeHabitavel,
                M3VolumeHabitavelCalculado = qualidadeDoAr.VolumeTotal,
                NumeroCompartimentos = orcamentoAr.NumeroCompartimentos,
                NumeroPisos = orcamentoAr.NumeroPisos,
                FiltroManutencao = orcamentoAr.FiltroManutencao,

                // Mapear checkboxes com base na existência das linhas de orçamento
                HasControleTecnico = getLinha("PROJ_CONTROLETECNICO") != null,
                HasExecucaoProjeto = getLinha("PROJ_EXECUCAOPROJETO") != null,
                HasInstalacaoMaoDeObra = getLinha("IMPL_MAODEOBRA") != null,
                HasAdaptacaoSistema = getLinha("PERS_ADAPTACAO") != null,
                HasAcessoriosExtras = getLinha("PERS_ACESSORIOS") != null,
                HasVigilancia24h = getLinha("MANUT_VIGILANCIA") != null,
            };

            return View(viewModel);
        }

        // Detalhe do Orçamento (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetalheOrcamento(DetalheOrcamentoViewModel model)
        {
            await SetQualidadeArSubmenuState(model.PropostaId, "DetalheOrcamento");

            if (!ModelState.IsValid)
            {
                var qualidadeDoAr = await _context.QualidadeDoAr.FindAsync(model.QualidadeDoArId);
                if (qualidadeDoAr != null) model.M3VolumeHabitavelCalculado = qualidadeDoAr.VolumeTotal;
                return View(model);
            }

            var orcamentoArParaAtualizar = await _context.OrcamentoAr
                .Include(oa => oa.LinhasOrcamento)
                .FirstOrDefaultAsync(oa => oa.Id == model.OrcamentoArId);

            if (orcamentoArParaAtualizar == null)
            {
                TempData["MensagemErro"] = "Orçamento não encontrado para atualização.";
                return NotFound();
            }

            // ... (dicionário de preços) ...

            // Atualizar os campos de dados principais
            orcamentoArParaAtualizar.M3VolumeHabitavel = model.M3VolumeHabitavel;
            orcamentoArParaAtualizar.NumeroCompartimentos = model.NumeroCompartimentos;
            orcamentoArParaAtualizar.NumeroPisos = model.NumeroPisos;

            // AQUI ESTÁ A CORREÇÃO: Garante que nunca é nulo
            orcamentoArParaAtualizar.FiltroManutencao = model.FiltroManutencao ?? "Nenhum"; // Se for nulo, atribui "Nenhum"

            // ... (restante do código que adiciona as linhas e salva) ...

            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Detalhes do orçamento guardados com sucesso!";
            return RedirectToAction("ResumoOrcamento", new { id = model.QualidadeDoArId });
        }


        [HttpGet]
        public async Task<IActionResult> ResumoOrcamento(ulong id) // id é o QualidadeDoArId
        {
            if (!ulong.TryParse(HttpContext.Session.GetString("CurrentPropostaId"), out ulong propostaId))
            {
                TempData["MensagemErro"] = "Sessão expirada. Por favor, retome o orçamento.";
                return RedirectToAction(nameof(OrçamentosEmCurso));
            }
            await SetQualidadeArSubmenuState(propostaId, "ResumoOrcamento");

            var proposta = await _context.Proposta.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.PropostaId == propostaId);
            if (proposta == null || !proposta.QualidadeDoArId.HasValue)
            {
                TempData["MensagemErro"] = "Orçamento de Qualidade do Ar não encontrado ou não iniciado.";
                return RedirectToAction(nameof(SelectTreatment));
            }

            var orcamentoAr = await _context.OrcamentoAr
                .Include(oa => oa.LinhasOrcamento)
                .FirstOrDefaultAsync(oa => oa.QualidadeDoAr.Id == id);

            if (orcamentoAr == null)
            {
                TempData["MensagemErro"] = "Detalhes do orçamento não encontrados.";
                return RedirectToAction("DetalheOrcamento", new { id });
            }

            // Agrupar e somar as linhas por categoria
            var categorias = orcamentoAr.LinhasOrcamento
                .GroupBy(l => l.CodigoItem.Split('_')[0]) // Agrupa por "PROJ", "FAB", "IMPL", etc.
                .Select(g => new ResumoCategoria
                {
                    Nome = MapearCodigoParaNomeCategoria(g.Key),
                    Montante = g.Sum(l => l.TotalLinha)
                })
                .ToList();

            decimal totalTributavel = categorias.Sum(c => c.Montante);
            decimal taxaIva = 6.0m; // Valor padrão, pode ser editável
            decimal valorIva = totalTributavel * (taxaIva / 100);
            decimal totalFinalComIva = totalTributavel + valorIva;

            // Lógica para a caixa azul (exemplo)
            string unidadesNecessarias = "1 unidade CTA 4"; // Substitua por sua lógica real

            var viewModel = new ResumoOrcamentoViewModel
            {
                PropostaId = propostaId,
                QualidadeDoArId = id,
                OrcamentoArId = orcamentoAr.Id,
                NomeCliente = $"{proposta.Cliente?.Nome} {proposta.Cliente?.Apelido}",
                Categorias = categorias,
                TotalTributavel = totalTributavel,
                TaxaIva = taxaIva,
                ValorIva = valorIva,
                TotalFinalComIva = totalFinalComIva,
                UnidadesNecessarias = unidadesNecessarias
            };

            return View(viewModel);
        }

        private string MapearCodigoParaNomeCategoria(string codigo)
        {
            return codigo switch
            {
                "PROJ" => "Projeto",
                "FAB" => "Fabricação",
                "IMPL" => "Instalação",
                "PERS" => "Personalização",
                "MANUT" => "Filtros", // Ou "Manutenção" se preferir
                _ => "Outros"
            };
        }

        // Resumo do Orçamento (POST) - para finalizar e guardar o orçamento
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResumoOrcamento(ResumoOrcamentoViewModel model)
        {
            await SetQualidadeArSubmenuState(model.PropostaId, "ResumoOrcamento");

            var orcamentoAr = await _context.OrcamentoAr.FindAsync(model.OrcamentoArId);
            if (orcamentoAr == null)
            {
                TempData["MensagemErro"] = "Orçamento não encontrado para finalizar.";
                return NotFound();
            }

            // Atualizar os valores finais no banco de dados
            orcamentoAr.TaxaIva = model.TaxaIva;
            orcamentoAr.ValorIva = model.ValorIva;
            orcamentoAr.TotalFinalComIva = model.TotalFinalComIva;

            // Opcional: Marcar a proposta como concluída
            var proposta = await _context.Proposta.FindAsync(model.PropostaId);
            if (proposta != null)
            {
                proposta.EstadoPropostaId = ESTADO_CONCLUIDO; // ID 2 = Concluído
            }

            await _context.SaveChangesAsync();

            TempData["MensagemSucesso"] = "Orçamento finalizado e guardado com sucesso!";
            return RedirectToAction("OrçamentosEmCurso"); // Ou para uma página de detalhes do orçamento concluído
        }
    }
}