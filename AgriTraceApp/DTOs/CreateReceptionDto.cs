namespace AgriTraceApp.DTOs
{
    public class CreateReceptionDto
    {

        public int EtiquetteFermeId { get; set; }
        public decimal PoidsBrut { get; set; }
        public decimal Temperature { get; set; }
        public string EtatProduit { get; set; } = string.Empty;
        public string TypeProduit { get; set; } = string.Empty;
        public string Observation { get; set; } = string.Empty;
        public string Utilisateur { get; set; } = string.Empty;

        // ✅ AJOUTER SocieteId
        public int SocieteId { get; set; } = 1;
    }
}
