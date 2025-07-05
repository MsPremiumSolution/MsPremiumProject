using System;
using System.Collections.Generic; // Adicionar este using para IEnumerable
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Adicionar este using para SelectListItem

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
        public string NomeCliente { get; set; } = string.Empty;

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
        public ulong? JanelaId { get; set; }

        [Display(Name = "Tipo de janela")]
        public string? TipoJanelaPrincipal { get; set; } // Este campo vai guardar o valor selecionado

        // PROPRIEDADE PARA PREENCHER O DROPDOWN:
        public IEnumerable<SelectListItem>? TiposJanelaDisponiveis { get; set; } // Lista de opções para o dropdown

        [Display(Name = "Material")]
        public string? MaterialJanela { get; set; }

        [Display(Name = "Janelas duplas")]
        public bool? JanelasDuplas { get; set; }

        [Display(Name = "Tipo de vidro")]
        public string? TipoVidro { get; set; }

        [Display(Name = "R.P.T.")]
        public bool? RPT { get; set; }

        [Display(Name = "Caixas de persiana")]
        public bool? CaixasPersiana { get; set; }

        [Display(Name = "Nº unidades")]
        public int? NumeroUnidadesJanela { get; set; }


        // ===============================================
        // CAMPOS DA TABELA Higrometria
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

        [Display(Name = "Temp. Ponto de Orvalho (ºC)")]
        public decimal? PontoDeOrvalho { get; set; }

        [Display(Name = "Temp. de Pontos Frios (ºC)")]
        public decimal? PontosFrios { get; set; }

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