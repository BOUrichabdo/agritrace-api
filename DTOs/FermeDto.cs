namespace TracAgriApi.DTOs
{
    public class FermeDto
    {

        public int Id { get; set; }

        public string NomFerme { get; set; } = string.Empty;

        public int AgriculteurId { get; set; }

        public string NomAgriculteur { get; set; } = string.Empty;
    }
}
