namespace TracAgriApi.Models
{
    public class SortieStock
    {

        public int Id { get; set; }

        // lien palette ou réception
        public int ReceptionId { get; set; }
        public Reception? Reception { get; set; }

        public decimal QuantiteSortie { get; set; }

        public DateTime DateSortie { get; set; }

        public string Utilisateur { get; set; } = string.Empty;
        public int SocieteId { get; set; }



        public string Observation { get; set; } = string.Empty;
    }
}
