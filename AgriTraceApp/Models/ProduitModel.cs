namespace AgriTraceApp.Models
{
    public class ProduitModel
    {

        public int Id { get; set; }

        // 🥦 Nom produit
        public string Nom { get; set; } = string.Empty;

        // 🌾 Parcelle
        public int ParcelleId { get; set; }
        public string? ParcelleNom { get; set; }

        // 🌱 Variété
        public int VarieteId { get; set; }

        public string? VarieteNom { get; set; }

        // 📦 Catégorie
        public string? CategorieNom { get; set; }
    }
}

