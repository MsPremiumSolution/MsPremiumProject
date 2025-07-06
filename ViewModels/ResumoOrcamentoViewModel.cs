// File: MSPremiumProject/ViewModels/ResumoOrcamentoViewModel.cs
using System.Collections.Generic;

namespace MSPremiumProject.ViewModels
{
    public class ResumoOrcamentoViewModel
    {
        public ulong PropostaId { get; set; }
        public ulong QualidadeDoArId { get; set; }
        public ulong OrcamentoArId { get; set; }
        public string NomeCliente { get; set; }

        public List<ResumoCategoria> Categorias { get; set; } = new List<ResumoCategoria>();

        public decimal TotalTributavel { get; set; }
        public decimal TaxaIva { get; set; }
        public decimal ValorIva { get; set; }
        public decimal TotalFinalComIva { get; set; }
        public string UnidadesNecessarias { get; set; } // Para a caixa azul
    }

    public class ResumoCategoria
    {
        public string Nome { get; set; }
        public decimal Montante { get; set; }
    }
}