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
                                 .Include(p => p.QualidadeDoAr)
                                     .ThenInclude(qa => qa.DadosGerais)
                                         .ThenInclude(dg => dg.Higrometria)
                                 .Include(p => p.QualidadeDoAr)
                                     .ThenInclude(qa => qa.DadosGerais)
                                         .ThenInclude(dg => dg.Sintomatologia)
                                 .FirstOrDefaultAsync(p => p.PropostaId == propostaId);

            if (proposta?.QualidadeDoAr != null)
            {
                stepsCompleted["ColecaoDados"] = proposta.QualidadeDoAr.DadosGeraisId.HasValue &&
                                                 proposta.QualidadeDoAr.DadosGerais?.DadosConstrutivo?.Id != 0 &&
                                                 proposta.QualidadeDoAr.DadosGerais?.Higrometria?.Id != 0 &&
                                                 proposta.QualidadeDoAr.DadosGerais?.Sintomatologia?.Id != 0;

                stepsCompleted["Objetivos"] = proposta.QualidadeDoAr.ObjetivosId.HasValue;
                stepsCompleted["Volumes"] = proposta.QualidadeDoAr.Volumes != null && proposta.QualidadeDoAr.Volumes.Any();

                stepsCompleted["DetalheOrcamento"] = stepsCompleted["ColecaoDados"] && stepsCompleted["Objetivos"] && stepsCompleted["Volumes"];
                stepsCompleted["ResumoOrcamento"] = stepsCompleted["DetalheOrcamento"];
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
            HttpContext.Session.Clear(); // Limpa a sessão ao iniciar um novo fluxo
            // ATENÇÃO: HttpContext.Session.Clear() aqui irá apagar 'CurrentPropostaId'
            // se o utilizador navegar para Index e depois voltar para um orçamento que estava a editar.
            // Considere manter o 'CurrentPropostaId' na sessão, se for para persistir entre navegações.

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

                // =========================================================================
                // LÓGICA ATUALIZADA: Limpeza de Propostas Incompletas Órfãs e Continuação
                // =========================================================================

                // 1. Encontrar todas as propostas "Em Curso" sem QualidadeDoArId para este Cliente/Utilizador
                var propostasIncompletas = await _context.Proposta
                    .Where(p => p.ClienteId == clienteId &&
                                p.UtilizadorId == utilizadorId &&
                                p.EstadoPropostaId == ESTADO_EM_CURSO &&
                                p.QualidadeDoArId == null) // Propostas em curso, mas sem a estrutura QA
                    .OrderByDescending(p => p.DataProposta) // Ordena para pegar a mais recente primeiro
                    .ToListAsync();

                Proposta propostaParaTrabalhar;

                if (propostasIncompletas.Any())
                {
                    // Existe pelo menos uma proposta incompleta.
                    propostaParaTrabalhar = propostasIncompletas.First(); // Escolhe a mais recente

                    // Remove as outras propostas incompletas "órfãs" (todas, exceto a que vamos trabalhar)
                    var propostasParaRemover = propostasIncompletas.Skip(1); // Todas menos a primeira/mais recente
                    if (propostasParaRemover.Any())
                    {
                        _context.Proposta.RemoveRange(propostasParaRemover);
                        await _context.SaveChangesAsync(); // Remove as propostas órfãs do DB
                        TempData["Debug: IniciarOrcamento - Cleaned Orphans"] = $"{propostasParaRemover.Count()} propostas órfãs foram limpas.";
                    }
                    TempData["Debug: IniciarOrcamento - Found Existing Proposal"] = $"Continuando proposta existente ID: {propostaParaTrabalhar.PropostaId}";
                }
                else
                {
                    // Não encontrou nenhuma proposta incompleta, cria uma nova
                    propostaParaTrabalhar = new Proposta
                    {
                        ClienteId = clienteId,
                        UtilizadorId = utilizadorId,
                        EstadoPropostaId = ESTADO_EM_CURSO,
                        DataProposta = DateTime.UtcNow
                    };
                    _context.Proposta.Add(propostaParaTrabalhar);
                    await _context.SaveChangesAsync(); // Salva a nova proposta para obter o ID
                    TempData["Debug: IniciarOrcamento - New Proposal Created"] = $"Nova proposta criada com ID: {propostaParaTrabalhar.PropostaId}";
                }

                HttpContext.Session.SetString("CurrentPropostaId", propostaParaTrabalhar.PropostaId.ToString());
                TempData["Debug: IniciarOrcamento - Session Set"] = $"Proposta ID {propostaParaTrabalhar.PropostaId} salva na sessão.";

                // Sempre redireciona para a Tipologia Construtiva
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
        public async Task<IActionResult> ContinuarOrcamento(ulong id)
        {
            var proposta = await _context.Proposta
                                     .Include(p => p.QualidadeDoAr)
                                     .FirstOrDefaultAsync(p => p.PropostaId == id);

            if (proposta == null || proposta.EstadoPropostaId != ESTADO_EM_CURSO)
            {
                TempData["MensagemErro"] = "Orçamento inválido ou já concluído.";
                return RedirectToAction(nameof(OrçamentosEmCurso));
            }

            HttpContext.Session.SetString("CurrentPropostaId", proposta.PropostaId.ToString());

            // Lógica para determinar onde continuar
            if (proposta.TipologiaConstrutivaId == null)
            {
                // Se a tipologia ainda não foi escolhida, vai para lá
                return RedirectToAction(nameof(TipologiaConstrutiva));
            }

            if (proposta.QualidadeDoArId == null)
            {
                // Se a tipologia foi escolhida, mas a estrutura de Qualidade do Ar não foi criada, vai para SelectTreatment
                return RedirectToAction(nameof(SelectTreatment));
            }

            // Se a estrutura de Qualidade do Ar já existe, vai para a página de edição de dados (Coleção de Dados)
            return RedirectToAction("EditQualidadeDoAr", new { id = proposta.QualidadeDoArId.Value });
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

            // Se QualidadeDoArId já tiver valor, significa que o tratamento já foi selecionado e a estrutura criada.
            // Redireciona para a edição de dados.
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
        // ... (resto do controlador) ...

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

            // ===============================================
            // BUSCAR TIPOS DE JANELA DA BASE DE DADOS
            // ===============================================
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
            hg.TemperaturaInterior = model.TemperaturaInterior;
            hg.TemperaturaParedesInternas = model.TemperaturaParedesInternas;
            hg.HumidadeRelativaInterior = model.HumidadeRelativaInterior; // Adicionei esta linha que estava faltando no seu código

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
            // MUDANÇA AQUI: Redirecionar para a ação Objetivos
            return RedirectToAction("Objetivos", new { id = model.QualidadeDoArId });
        }


        [HttpGet]
        public async Task<IActionResult> Objetivos(ulong id) // O 'id' aqui será o QualidadeDoArId
        {
            if (!ulong.TryParse(HttpContext.Session.GetString("CurrentPropostaId"), out ulong propostaId))
            {
                TempData["MensagemErro"] = "Sessão expirada. Por favor, retome o orçamento.";
                return RedirectToAction(nameof(OrçamentosEmCurso));
            }

            // Definir o estado do submenu para "Objetivos"
            await SetQualidadeArSubmenuState(propostaId, "Objetivos");

            var proposta = await _context.Proposta.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.PropostaId == propostaId);
            if (proposta == null)
            {
                TempData["MensagemErro"] = "Proposta associada ao orçamento de Qualidade do Ar não encontrada.";
                return NotFound();
            }

            // Buscar a entidade QualidadeDoAr e seus Objetivos associados
            var qualidadeDoAr = await _context.QualidadeDoAr
                .Include(qa => qa.Objetivos)
                .AsNoTracking() // Usamos AsNoTracking para evitar rastreamento desnecessário se for apenas para exibição
                .FirstOrDefaultAsync(qa => qa.Id == id);

            if (qualidadeDoAr == null || qualidadeDoAr.Objetivos == null)
            {
                TempData["MensagemErro"] = "A estrutura de objetivos para este orçamento não foi encontrada ou está incompleta. Por favor, reinicie a criação do orçamento ou contacte o suporte.";
                return RedirectToAction(nameof(SelectTreatment)); // Ou outra ação apropriada
            }

            ViewData["Title"] = $"Objetivos - {proposta.Cliente.Nome} {proposta.Cliente.Apelido}";

            // Mapear os dados do modelo para o ViewModel
            var viewModel = new ObjetivosViewModel
            {
                PropostaId = propostaId,
                QualidadeDoArId = qualidadeDoAr.Id,
                NomeCliente = $"{proposta.Cliente.Nome} {proposta.Cliente.Apelido}",

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

            return View(viewModel);
        }

        // Implementação do método POST para salvar os Objetivos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Objetivos(ObjetivosViewModel model)
        {
            // Definir o estado do submenu para "Objetivos"
            await SetQualidadeArSubmenuState(model.PropostaId, "Objetivos");

            if (!ModelState.IsValid)
            {
                // Se houver erros de validação, recarregar o nome do cliente e retornar a view
                var propostaCliente = await _context.Proposta.Include(p => p.Cliente).AsNoTracking().FirstOrDefaultAsync(p => p.PropostaId == model.PropostaId);
                if (propostaCliente != null)
                {
                    model.NomeCliente = $"{propostaCliente.Cliente.Nome} {propostaCliente.Cliente.Apelido}";
                }
                ViewData["Title"] = $"Objetivos - {model.NomeCliente}"; // Manter o título correto
                return View(model);
            }

            // Buscar a entidade QualidadeDoAr com seus Objetivos para atualização
            var qualidadeDoArParaAtualizar = await _context.QualidadeDoAr
                .Include(qa => qa.Objetivos)
                .FirstOrDefaultAsync(qa => qa.Id == model.QualidadeDoArId);

            if (qualidadeDoArParaAtualizar == null || qualidadeDoArParaAtualizar.Objetivos == null)
            {
                TempData["MensagemErro"] = "Dados de objetivos incompletos ou não encontrados para atualização.";
                return NotFound();
            }

            var objetivos = qualidadeDoArParaAtualizar.Objetivos;

            // Atualizar as propriedades do modelo Objetivos com os dados do ViewModel
            objetivos.IsolamentoExternoSATE = model.IsolamentoExternoSATE;
            objetivos.IsolamentoInteriorPladur = model.IsolamentoInteriorPladur;
            objetivos.InjeccaoCamaraArPoliuretano = model.InjeccaoCamaraArPoliuretano;
            objetivos.TrituracaoCorticaTriturada = model.TrituracaoCorticaTriturada;
            objetivos.AplicacaoTintaTermica = model.AplicacaoTintaTermica;
            objetivos.ImpermeabilizacaoFachadas = model.ImpermeabilizacaoFachadas;
            objetivos.TubagemParedesInfiltracao = model.TubagemParedesInfiltracao;
            objetivos.InjeccaoParedesAccaoCapilar = model.InjeccaoParedesAccaoCapilar;
            objetivos.EvacuacaoHumidadeExcesso = model.EvacuacaoHumidadeExcesso;

            await _context.SaveChangesAsync();
            TempData["MensagemSucesso"] = "Objetivos guardados com sucesso!";

            // Redirecionar para a próxima etapa (por exemplo, Volumes)
            return RedirectToAction("Volumes", new { id = model.QualidadeDoArId });
        }

        //================================================================================
        // PÁGINAS PLACEHOLDER PARA AS OUTRAS SUB-ETAPAS DA QUALIDADE DO AR
        // Todas ativam o submenu e o link correto
        //================================================================================
        

        [HttpGet]
        public async Task<IActionResult> Volumes()
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
            await SetQualidadeArSubmenuState(propostaId, "Volumes");

            ViewData["Title"] = "Volumes";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DetalheOrcamento()
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
            await SetQualidadeArSubmenuState(propostaId, "DetalheOrcamento");

            ViewData["Title"] = "Detalhe do Orçamento";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ResumoOrcamento()
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
            await SetQualidadeArSubmenuState(propostaId, "ResumoOrcamento");

            ViewData["Title"] = "Resumo do Orçamento";
            return View();
        }

        //================================================================================
        // AÇÃO PARA APAGAR PROPOSTAS EM CURSO
        //================================================================================
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
                // Lógica de eliminação em cascata para QualidadeDoAr e suas dependências.
                // Esta lógica só é estritamente necessária se o seu banco de dados NÃO tiver
                // ON DELETE CASCADE configurado para as chaves estrangeiras.
                // Se o seu DB tiver, remover a Proposta pode já ser suficiente.
                if (proposta.QualidadeDoArId.HasValue)
                {
                    var qualidadeAr = await _context.QualidadeDoAr
                        .Include(qa => qa.DadosGerais)
                            .ThenInclude(dg => dg.DadosConstrutivo)
                                .ThenInclude(dc => dc.Janelas) // Garante que as janelas são carregadas para serem removidas
                        .Include(qa => qa.DadosGerais)
                            .ThenInclude(dg => dg.Higrometria)
                        .Include(qa => qa.DadosGerais)
                            .ThenInclude(dg => dg.Sintomatologia)
                        .Include(qa => qa.Objetivos)
                        .Include(qa => qa.OrcamentoAr)
                        .FirstOrDefaultAsync(qa => qa.Id == proposta.QualidadeDoArId.Value);

                    if (qualidadeAr != null)
                    {
                        if (qualidadeAr.DadosGerais != null)
                        {
                            if (qualidadeAr.DadosGerais.DadosConstrutivo != null)
                            {
                                // Remove todas as janelas associadas a este DadosConstrutivos
                                _context.Janelas.RemoveRange(qualidadeAr.DadosGerais.DadosConstrutivo.Janelas);
                                _context.DadosConstrutivos.Remove(qualidadeAr.DadosGerais.DadosConstrutivo);
                            }
                            if (qualidadeAr.DadosGerais.Higrometria != null) _context.Higrometria.Remove(qualidadeAr.DadosGerais.Higrometria);
                            if (qualidadeAr.DadosGerais.Sintomatologia != null) _context.Sintomatologia.Remove(qualidadeAr.DadosGerais.Sintomatologia);
                            _context.DadosGerais.Remove(qualidadeAr.DadosGerais);
                        }
                        if (qualidadeAr.Objetivos != null) _context.Objetivos.Remove(qualidadeAr.Objetivos);
                        if (qualidadeAr.OrcamentoAr != null) _context.OrcamentoAr.Remove(qualidadeAr.OrcamentoAr);
                        _context.QualidadeDoAr.Remove(qualidadeAr);
                    }
                }

                _context.Proposta.Remove(proposta);
                await _context.SaveChangesAsync();
                TempData["MensagemSucesso"] = $"A proposta Nº {id} foi apagada com sucesso.";
            }
            catch (DbUpdateException ex)
            {
                TempData["MensagemErro"] = $"Ocorreu um erro ao apagar a proposta: Por favor, verifique se todas as informações associadas (dados de qualidade do ar, etc.) foram removidas ou se há referências pendentes. Detalhes: {ex.Message}";
                // Considere logar ex.InnerException para mais detalhes em depuração
            }

            return RedirectToAction(nameof(OrçamentosEmCurso));
        }
    }
}