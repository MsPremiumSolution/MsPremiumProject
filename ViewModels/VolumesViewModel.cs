// File: MSPremiumProject/ViewModels/VolumesViewModel.cs
using System.Collections.Generic;

namespace MSPremiumProject.ViewModels
{
    public class VolumesViewModel
    {
        public ulong PropostaId { get; set; }
        public ulong QualidadeDoArId { get; set; }
        //public string NomeCliente { get; set; }

        public List<VolumeItemViewModel> Volumes { get; set; } = new List<VolumeItemViewModel>();
    }

    public class VolumeItemViewModel
    {
        public ulong Id { get; set; }
        public decimal Altura { get; set; }
        public List<MedidaItemViewModel> Medidas { get; set; } = new List<MedidaItemViewModel>();
    }

    public class MedidaItemViewModel
    {
        public ulong Id { get; set; }
        public decimal Largura { get; set; }
        public decimal Comprimento { get; set; }
    }
}