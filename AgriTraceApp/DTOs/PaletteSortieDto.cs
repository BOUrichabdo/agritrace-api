namespace AgriTraceApp.DTOs
{
    public class PaletteSortieDto
    {
        public string CodePalette { get; set; } = string.Empty;

        public string Produit { get; set; } = string.Empty;

        public string Variete { get; set; } = string.Empty;

        public decimal QuantiteDisponible { get; set; }

        public string EtatPalette { get; set; } = string.Empty;
    }
}
