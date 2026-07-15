namespace AgriTraceApp.Models
{
    public class ModeleVariete
    {
        public int Id { get; set; }

        public string Intitule { get; set; } = string.Empty;

        public int CategorieId { get; set; }

        public string CategorieNom { get; set; }
    }
}


