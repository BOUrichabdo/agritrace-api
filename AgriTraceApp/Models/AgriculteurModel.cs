namespace AgriTraceApp.Models
{
    public class AgriculteurModel
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;

        public int SocieteId { get; set; }

    }
}
