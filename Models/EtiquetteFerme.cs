namespace TracAgriApi.Models
{
    public class EtiquetteFerme
    {

        public int Id { get; set; }

        // 🔢 code unique QR  code produit 
        public string CodeEtiquette { get; set; } = string.Empty;

        // =========================
        // RELATIONS
        // =========================

        public int AgriculteurId { get; set; }

        public int FermeId { get; set; }

        public int ParcelleId { get; set; }

        public int ProduitId { get; set; }

        public int VarieteId { get; set; }

        // =========================
        // INFOS TRACEABILITÉ
        // =========================

        public DateTime DateGeneration { get; set; }

        public bool Receptionne { get; set; }

        public DateTime? DateReception { get; set; }

        // =========================
        // NAVIGATION
        // =========================
                public int SocieteId { get; set; }

        public Agriculteur? Agriculteur { get; set; }

        public Ferme? Ferme { get; set; }

        public Parcelle? Parcelle { get; set; }

        public Produit? Produit { get; set; }

        public Variete? Variete { get; set; }
    }
}
