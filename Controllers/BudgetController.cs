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

            var proposta = await _context.Proposta // Certifica-te que está "Propostas" aqui
                                 .Include(p => p.QualidadeDoAr)
                                     .ThenInclude(qa => qa.DadosGerais)
                                         .ThenInclude(dg => dg.DadosConstrutivo)
                                 .Include(p => p.QualidadeDoAr)
                                     .ThenInclude(qa => qa.Objetivos)
                                 .Include(p => p.QualidadeDoAr)
                                     .ThenInclude(qa => qa.Volumes) // Certifica-te que 'Volumes' é uma coleção no modelo QualidadeDoAr
                                 .Include(p => p.QualidadeDoAr)
                                     .ThenInclude(qa => qa.DadosGerais)
                                         .ThenInclude(dg => dg.Higrometria)
                                 .Include(p => p.QualidadeDoAr)
                                     .ThenInclude(qa => qa.DadosGerais)
                                         .ThenInclude(dg => dg.Sintomatologia)
                                 .FirstOrDefaultAsync(p => p.PropostaId == propostaId);

            if (proposta?.QualidadeDoAr != null)
            {
                // Coleção de Dados: Considerado completo se DadosGerais e suas sub-partes existirem
                // Verifica se o ID das FKs são diferentes de 0 (se não forem nullable) ou se têm valor (se forem nullable)
                stepsCompleted["ColecaoDados"] = proposta.QualidadeDoAr.DadosGeraisId.HasValue && // Se DadosGeraisId é nullable
                                                 proposta.QualidadeDoAr.DadosGerais?.DadosConstrutivo?.Id != 0 &&
                                                 proposta.QualidadeDoAr.DadosGerais?.Higrometria?.Id != 0 &&
                                                 proposta.QualidadeDoAr.DadosGerais?.Sintomatologia?.Id != 0;

                stepsCompleted["Objetivos"] = proposta.QualidadeDoAr.ObjetivosId.HasValue; // Se ObjetivosId é nullable
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
                EstadoPropostaId = ESTADO_EM_CURSO, // 1 = "Em Curso"
                DataProposta = DateTime.UtcNow
            };

            _context.Proposta.Add(novaProposta);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("CurrentPropostaId", novaProposta.PropostaId.ToString());

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

            proposta = await _context.Proposta
                                     .Include(p => p.QualidadeDoAr)
                                     .FirstOrDefaultAsync(p => p.PropostaId == id);

            if (proposta.QualidadeDoArId.HasValue)
            {
                return RedirectToAction("EditQualidadeDoAr", new { id = proposta.QualidadeDoArId.Value });
            }

            if (proposta.TipologiaConstrutivaId == null)
            {
                return RedirectToAction(nameof(TipologiaConstrutiva));
            }
            return RedirectToAction(nameof(SelectTreatment));
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
                await SetQualidadeArSubmenuState(propostaId, "TipologiaConstrutiva");
            }

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
            await SetQualidadeArSubmenuState(id, "ColecaoDados");

            if (!ulong.TryParse(HttpContext.Session.GetString("CurrentPropostaId"), out ulong propostaId))
            {
                TempData["MensagemErro"] = "Sessão expirada.";
                return RedirectToAction(nameof(OrçamentosEmCurso));
            }

            var proposta = await _context.Proposta.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.PropostaId == propostaId);
            if (proposta == null) return NotFound();

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

            if (tratamento?.DadosGerais?.DadosConstrutivo == null) return NotFound("A estrutura de dados para este orçamento não foi encontrada ou está incompleta.");

            var primeiraJanela = tratamento.DadosGerais.DadosConstrutivo.Janelas?.FirstOrDefault();

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
                TemperaturaPontoOrvalho = tratamento.DadosGerais.Higrometria.TemperaturaPontoOrvalho,
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

            if (tratamentoParaAtualizar == null) return NotFound();

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

            // CORREÇÃO AQUI: Garante que 'TratamentoHidrofugacao' (bool?) se converte para 'bool'
            dc.TratamentoHidrofugacao = model.TratamentoHidrofugacao ?? false;

            dc.IsolamentoCamara = model.IsolamentoCamara;
            dc.IsolamentoInterno = model.IsolamentoInterno;
            dc.TipoAquecimento = model.TipoAquecimento;

            // Lógica para Salvar/Atualizar a PRIMEIRA Janela
            Janela janelaParaAtualizar;

            if (model.JanelaId.HasValue) // Se já existe um ID de Janela, tenta carregá-la
            {
                janelaParaAtualizar = dc.Janelas.FirstOrDefault(j => j.Id == model.JanelaId.Value);
                if (janelaParaAtualizar == null)
                {
                    janelaParaAtualizar = new Janela { DadosConstrutivosId = dc.Id };
                    dc.Janelas.Add(janelaParaAtualizar);
                }
            }
            else
            {
                // Verifica se pelo menos um campo da janela foi preenchido para evitar criar janelas vazias
                // Também inclui as booleanas da Janela na condição
                if (!string.IsNullOrEmpty(model.TipoJanelaPrincipal) || model.NumeroUnidadesJanela.HasValue || model.JanelasDuplas.HasValue || model.RPT.HasValue || model.CaixasPersiana.HasValue)
                {
                    janelaParaAtualizar = new Janela { DadosConstrutivosId = dc.Id };
                    dc.Janelas.Add(janelaParaAtualizar);
                }
                else
                {
                    janelaParaAtualizar = null;
                }
            }

            if (janelaParaAtualizar != null)
            {
                janelaParaAtualizar.TipoJanela = model.TipoJanelaPrincipal;
                janelaParaAtualizar.Material = model.MaterialJanela;
                janelaParaAtualizar.TipoVidro = model.TipoVidro;
                janelaParaAtualizar.NumeroUnidades = model.NumeroUnidadesJanela;
                // CORREÇÃO AQUI: Garante que 'bool?' se convertem para 'bool' se necessário
                janelaParaAtualizar.PossuiJanelasDuplas = model.JanelasDuplas ?? false;
                janelaParaAtualizar.PossuiRPT = model.RPT ?? false;
                janelaParaAtualizar.PossuiCaixaPersiana = model.CaixasPersiana ?? false;
            }


            // Atualizar Higrometria
            var hg = tratamentoParaAtualizar.DadosGerais.Higrometria;
            hg.HumidadeRelativaExterior = model.HumidadeRelativaExterior; hg.TemperaturaExterior = model.TemperaturaExterior; hg.HumidadeRelativaInterior = model.HumidadeRelativaInterior; hg.TemperaturaInterior = model.TemperaturaInterior; hg.TemperaturaParedesInternas = model.TemperaturaParedesInternas; hg.TemperaturaPontoOrvalho = model.TemperaturaPontoOrvalho;
            hg.PontoDeOrvalho = model.PontoDeOrvalho;
            // CORREÇÃO AQUI: Garante que 'PontosFrios' (bool?) se converte para 'bool'
            //hg.PontosFrios = model.PontosFrios ?? false;

            hg.NivelCO2 = model.NivelCO2; hg.NivelTCOV = model.NivelTCOV; hg.NivelHCHO = model.NivelHCHO; hg.DataLoggerSensores = model.DataLoggerSensores;

            // Atualizar Sintomatologia
            var st = tratamentoParaAtualizar.DadosGerais.Sintomatologia;
            // CORREÇÃO AQUI para todas as booleanas de Sintomatologia
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
            return RedirectToAction("EditQualidadeDoAr", new { id = model.QualidadeDoArId });
        }


        //================================================================================
        // PÁGINAS PLACEHOLDER PARA AS OUTRAS SUB-ETAPAS DA QUALIDADE DO AR
        // Todas ativam o submenu e o link correto
        //================================================================================
        [HttpGet]
        public async Task<IActionResult> Objetivos()
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
            await SetQualidadeArSubmenuState(propostaId, "Objetivos");

            ViewData["Title"] = "Objetivos";
            return View();
        }

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