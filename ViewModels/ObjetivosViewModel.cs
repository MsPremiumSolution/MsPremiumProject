// File: MSPremiumProject/ViewModels/ObjetivosViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace MSPremiumProject.ViewModels
{
    public class ObjetivosViewModel
    {
        public ulong PropostaId { get; set; }
        public ulong QualidadeDoArId { get; set; }
        

        // --- Possíveis tratamentos ---
        [Display(Name = "Isolamento externo com S.A.T.E.")]
        public bool IsolamentoExternoSATE { get; set; }

        [Display(Name = "Isolamento interior com placas de gesso cartonado laminado (Pladur)")]
        public bool IsolamentoInteriorPladur { get; set; }

        [Display(Name = "Injeção de câmaras de ar de poliuretano")]
        public bool InjeccaoCamaraArPoliuretano { get; set; }

        [Display(Name = "Trituração/moagem de cortiça triturada")]
        public bool TrituracaoCorticaTriturada { get; set; }

        [Display(Name = "Aplicação de tinta térmica")]
        public bool AplicacaoTintaTermica { get; set; }

        [Display(Name = "Impermeabilização de fachadas")]
        public bool ImpermeabilizacaoFachadas { get; set; }

        [Display(Name = "Tubagem das paredes afetadas pela infiltração")]
        public bool TubagemParedesInfiltracao { get; set; }

        [Display(Name = "Injeção de paredes afetadas pela acção capilar")]
        public bool InjeccaoParedesAccaoCapilar { get; set; }

        [Display(Name = "Evacuação da humidade ambiente em excesso")]
        public bool EvacuacaoHumidadeExcesso { get; set; }

        // As propriedades para "Objetivos a alcançar" foram removidas.
    }
}