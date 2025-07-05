using System;
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.ViewModels
{
    /// <summary>
    /// ViewModel que agrega todos os dados para a página "Coleção de Dados" do orçamento de Qualidade do Ar.
    /// Combina campos das entidades DadosConstrutivos, Higrometria e Sintomatologia.
    /// </summary>
    public class QualidadeArViewModel
    {
        // --- IDs e Dados de Contexto ---
        public ulong PropostaId { get; set; }
        public ulong QualidadeDoArId { get; set; }
        public string NomeCliente { get; set; } = string.Empty; // Inicializado para evitar null reference

        // ===============================================
        // CAMPOS DA TABELA DadosConstrutivos
        // ===============================================
        [Display(Name = "Data da Visita")]
        [DataType(DataType.Date)]
        public DateTime? DataVisita { get; set; }

        [Display(Name = "Ano de Construção")]
        public int? AnoConstrucao { get; set; }

        [Display(Name = "Área (m²)")]
        public decimal? AreaM2 { get; set; }

        [Display(Name = "Nº de Andares")]
        public int? NumeroAndares { get; set; }

        [Display(Name = "Nº de Habitantes")]
        public int? NumeroHabitantes { get; set; }

        [Display(Name = "Localidade")]
        public string? Localidade { get; set; }

        [Display(Name = "Altitude (m)")]
        public int? Altitude { get; set; }

        [Display(Name = "Tipo de fachada")]
        public string? TipoFachada { get; set; }

        [Display(Name = "Orientação da fachada")]
        public string? OrientacaoFachada { get; set; }

        [Display(Name = "Cobertura da fachada principal")]
        public string? CoberturaFachadaPrincipal { get; set; }

        [Display(Name = "Cobertura da fachada posterior")]
        public string? CoberturaFachadaPosterior { get; set; }

        [Display(Name = "Tratamento de hidrofugação")]
        public bool? TratamentoHidrofugacao { get; set; }

        [Display(Name = "Isolamento da câmara")]
        public string? IsolamentoCamara { get; set; }

        [Display(Name = "Isolamento interno")]
        public string? IsolamentoInterno { get; set; }

        [Display(Name = "Tipo de aquecimento")]
        public string? TipoAquecimento { get; set; }

        // ===============================================
        // CAMPOS DA TABELA Janela (para a primeira janela)
        // ===============================================
        public ulong? JanelaId { get; set; } // ID da entidade Janela que estamos a editar/criar

        [Display(Name = "Tipo de janela")]
        public string? TipoJanelaPrincipal { get; set; } // Mapeia para Janela.TipoJanela

        [Display(Name = "Material")]
        public string? MaterialJanela { get; set; } // Mapeia para Janela.Material

        [Display(Name = "Janelas duplas")]
        public bool? JanelasDuplas { get; set; } // Mapeia para Janela.PossuiJanelasDuplas

        [Display(Name = "Tipo de vidro")]
        public string? TipoVidro { get; set; } // Mapeia para Janela.TipoVidro

        [Display(Name = "R.P.T.")]
        public bool? RPT { get; set; } // Mapeia para Janela.PossuiRPT

        [Display(Name = "Caixas de persiana")]
        public bool? CaixasPersiana { get; set; } // Mapeia para Janela.PossuiCaixaPersiana

        [Display(Name = "Nº unidades")]
        public int? NumeroUnidadesJanela { get; set; } // Mapeia para Janela.NumeroUnidades


        // ===============================================
        // CAMPOS DA TABELA Higrometria
        // Re-purposing existing model fields based on user request.
        // ===============================================
        [Display(Name = "Humidade Relativa Exterior (%)")]
        public decimal? HumidadeRelativaExterior { get; set; }

        [Display(Name = "Temperatura Exterior (ºC)")]
        public decimal? TemperaturaExterior { get; set; }

        [Display(Name = "Humidade Relativa Interior (%)")]
        public decimal? HumidadeRelativaInterior { get; set; }

        [Display(Name = "Temperatura Interior (ºC)")]
        public decimal? TemperaturaInterior { get; set; }

        [Display(Name = "Temp. Paredes Internas (ºC)")]
        public decimal? TemperaturaParedesInternas { get; set; }

        // ESTE CAMPO NO MODELO HIGROMETRIA É 'TemperaturaPontoOrvalho' (decimal?)
        // Mas na UI/ViewModel, representará a TEMPERATURA DOS PONTOS FRIOS
        [Display(Name = "Temp. de Pontos Frios (ºC)")]
        public decimal? TemperaturaDePontosFrios { get; set; }

        // ESTE CAMPO NO MODELO HIGROMETRIA É 'PontoDeOrvalho' (decimal?)
        // Mas na UI/ViewModel, representará a TEMPERATURA DO PONTO DE ORVALHO
        [Display(Name = "Temp. Ponto de Orvalho (ºC)")]
        public decimal? PontoDeOrvalho { get; set; }

        // ESTE CAMPO NO MODELO HIGROMETRIA É 'PontosFrios' (decimal?)
        // Mas na UI/ViewModel, representa a PRESENÇA de Pontos Frios (Sim/Não)
        [Display(Name = "Pontos Frios (Presença)")]
        public decimal? PontosFrios { get; set; } // O ViewModel ainda usa bool? para o dropdown "Sim/Não"

        [Display(Name = "Nível CO² (ppm)")]
        public decimal? NivelCO2 { get; set; }

        [Display(Name = "Nível TCOV (mg/m³)")]
        public decimal? NivelTCOV { get; set; }

        [Display(Name = "Nível HCHO (mg/m³)")]
        public decimal? NivelHCHO { get; set; }

        [Display(Name = "Data Logger Sensores")]
        public decimal? DataLoggerSensores { get; set; }

        // ===============================================
        // CAMPOS DA TABELA Sintomatologia
        // ===============================================
        [Display(Name = "Presença de fungos")]
        public bool? Fungos { get; set; }

        [Display(Name = "Presença de cheiros (mofo)")]
        public bool? Cheiros { get; set; }

        [Display(Name = "Mofo em roupas/armários")]
        public bool? MofoEmRoupasArmarios { get; set; }

        [Display(Name = "Condensação nas janelas")]
        public bool? CondensacaoNasJanelas { get; set; }

        [Display(Name = "Consumo excessivo de aquecimento")]
        public bool? ConsumoExcessivoAquecimento { get; set; }

        [Display(Name = "Presença de alergias nos habitantes")]
        public bool? Alergias { get; set; }

        [Display(Name = "Problemas respiratórios nos habitantes")]
        public bool? ProblemasRespiratorios { get; set; }

        [Display(Name = "Suspeita de Gás Radão")]
        public bool? GasRadao { get; set; }

        [Display(Name = "Esporos em superfícies")]
        public bool? EsporosEmSuperficies { get; set; }
    }
}