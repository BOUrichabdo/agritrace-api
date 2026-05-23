namespace TracAgriApi.DTOs
{
    public class ReceptionResponseDto
    {
        public int ReceptionId { get; set; }
        public int PaletteId { get; set; }
        public string CodePalette { get; set; } = string.Empty;

        // Informations réception
        public decimal PoidsBrut { get; set; }
        public decimal QuantiteDisponible { get; set; }
        public DateTime DateReception { get; set; }
        public decimal Temperature { get; set; }
        public string Etat { get; set; } = string.Empty;        // Bon/Moyen/Mauvais
        public string Type { get; set; } = string.Empty;        // Produit Fini/Matière Première
        public string Observation { get; set; } = string.Empty;

        // Informations étiquette ferme
        public string CodeQR { get; set; } = string.Empty;
        public string Agriculteur { get; set; } = string.Empty;
        public string Ferme { get; set; } = string.Empty;
        public string Produit { get; set; } = string.Empty;
        public string Variete { get; set; } = string.Empty;
    }
}