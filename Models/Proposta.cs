// Ficheiro: Models/Proposta.cs

using MSPremiumProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSPremiumProject.Models;

[Table("Proposta")] // Nome da tabela na base de dados
public partial class Proposta
{
    [Key]
    public ulong PropostaId { get; set; }

    public ulong ClienteId { get; set; }

    public DateTime DataProposta { get; set; }

    public DateTime DataAceitacao { get; set; }

    /// <summary>
    /// Codigo composto por AAAAMMMDD0000PropostaID
    /// </summary>
    // Torna a propriedade nullable com um '?'
    public string? CodigoProposta { get; set; }

    public ulong UtilizadorId { get; set; }

    public int EstadoPropostaId { get; set; }
    public ulong? TipologiaConstrutivaId { get; set; }

    public decimal ValorObra { get; set; }

    // --- NOVAS COLUNAS DE CHAVE ESTRANGEIRA ---
    public ulong? QualidadeDoArId { get; set; }
    public ulong? TratamentoEstruturalId { get; set; }


    // --- RELAÇÕES DE NAVEGAÇÃO ATUALIZADAS ---

    // --- NOVA PROPRIEDADE DE NAVEGAÇÃO ---
    [ForeignKey("TipologiaConstrutivaId")]
    public virtual TipologiaConstrutiva TipologiaConstrutiva { get; set; } = null!;

    // Relação com Cliente (Muitas Propostas para Um Cliente)
    [ForeignKey("ClienteId")]
    public virtual Cliente Cliente { get; set; } = null!;

    // Relação com Utilizador (Muitas Propostas para Um Utilizador)
    [ForeignKey("UtilizadorId")]
    public virtual Utilizador Utilizador { get; set; } = null!;

    // Relação com QualidadeDoAr (Uma Proposta para Uma QualidadeDoAr)
    [ForeignKey("QualidadeDoArId")]
    public virtual QualidadeDoAr? QualidadeDoAr { get; set; }

    [ForeignKey("EstadoPropostaId")]
    public virtual EstadoProposta Estado { get; set; } = null!; // A propriedade de navegação


}