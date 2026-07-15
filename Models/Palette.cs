namespace TracAgriApi.Models
{
    public class Palette
    {

        public int Id { get; set; }

        public string CodePalette { get; set; } = string.Empty;

        public int ProduitId { get; set; }

        public Produit? Produit { get; set; }

        public decimal PoidsBrut { get; set; }



        public decimal QuantiteDisponible { get; set; }

        public string EtatPalette { get; set; } = string.Empty;

        public string? StatutStock
        {
            get; set;
        }

        public DateTime DateCreation { get; set; }

        // Localisation
        public string Emplacement { get; set; } = string.Empty;

        // Traçabilité
        public int ReceptionId { get; set; }

        public int SocieteId { get; set; }


        public Reception? Reception { get; set; }
    }
}
