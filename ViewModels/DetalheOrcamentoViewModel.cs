// File: MSPremiumProject/ViewModels/DetalheOrcamentoViewModel.cs
namespace MSPremiumProject.ViewModels
{
    public class DetalheOrcamentoViewModel
    {
        public ulong PropostaId { get; set; }
        public ulong QualidadeDoArId { get; set; }
        public ulong OrcamentoArId { get; set; }
        public string NomeCliente { get; set; }

        // Seção Projeto
        public bool HasControleTecnico { get; set; }
        public bool HasExecucaoProjeto { get; set; }

        // Seção Fabricação
        public decimal M3VolumeHabitavel { get; set; }
        public decimal M3VolumeHabitavelCalculado { get; set; } // Valor vindo do passo "Volumes"
        public int NumeroCompartimentos { get; set; }
        public int NumeroPisos { get; set; }

        // Seção Implementação
        public bool HasInstalacaoMaoDeObra { get; set; }

        // Seção Personalização
        public bool HasAdaptacaoSistema { get; set; }
        public bool HasAcessoriosExtras { get; set; }

        // Seção Manutenção
        public string FiltroManutencao { get; set; } // "G4" ou "F7"
        public bool HasVigilancia24h { get; set; }
    }
}