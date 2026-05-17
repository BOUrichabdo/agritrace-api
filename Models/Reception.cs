namespace TracAgriApi.Models
{
    public class Reception
    {

        public int Id { get; set; }

        // Etiquette ferme scannée
        public int EtiquetteFermeId { get; set; }

        public EtiquetteFerme? EtiquetteFerme { get; set; }

        // Infos réception
        public decimal PoidsBrut { get; set; }

        public decimal Temperature { get; set; }

        public string EtatProduit { get; set; } = string.Empty;

        public string TypeProduit { get; set; } = string.Empty;

        public string Observation { get; set; } = string.Empty;

        // Palette créée après réception
        public int? PaletteId { get; set; }

        public Palette? Palette { get; set; }

        // Audit
        public DateTime DateReception { get; set; }

        public int SocieteId { get; set; }


        public string Utilisateur { get; set; } = string.Empty;
    }
}
