namespace AgriTraceApp.Models
{
    public class FermeModele
    {
        public int Id { get; set; }
        public string NomFerme { get; set; } = string.Empty;

        public int AgriculteurId { get; set; }

        public AgriculteurModel? Agriculteur { get; set; }


    }
}
