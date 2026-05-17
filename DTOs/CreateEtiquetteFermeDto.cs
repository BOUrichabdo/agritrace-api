namespace TracAgriApi.DTOs
{
    public class CreateEtiquetteFermeDto
    {

        public int AgriculteurId { get; set; }

        public int FermeId { get; set; }

        public int ParcelleId { get; set; }

        public int ProduitId { get; set; }

        public int VarieteId { get; set; }

        public string CodeEtiquette { get; set; } = string.Empty; 



        
    }
}
