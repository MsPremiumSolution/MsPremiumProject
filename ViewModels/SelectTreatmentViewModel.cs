namespace MSPremiumProject.ViewModels
{
    public class SelectTreatmentViewModel
    {
        // Usamos ulong porque o teu ClienteId é ulong
        public ulong ClienteId { get; set; }
        public string NomeCliente { get; set; }
    }
}