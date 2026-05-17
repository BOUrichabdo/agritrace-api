namespace TracAgriApi.DTOs
{
    public class VarieteDto
    {
        public int Id { get; set; }
        public string Intitule { get; set; } = string.Empty;
        public int CategorieId { get; set; }
        public string? CategorieNom { get; set; }
    }
}
