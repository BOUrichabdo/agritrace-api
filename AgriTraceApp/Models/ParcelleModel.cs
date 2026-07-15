namespace AgriTraceApp.Models
{
    public class ParcelleModel
    {
        public int Id { get; set; }

        public string NomParcelle { get; set; } = string.Empty;

        // 🔑 FOREIGN KEY → Ferme
        public int FermeId { get; set; }

        // 🔗 navigation (optionnel côté MAUI UI)
        public FermeModele? Ferme { get; set; }
    }
}
