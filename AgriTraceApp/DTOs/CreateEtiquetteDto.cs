namespace AgriTraceApp.DTOs
{
    public class CreateEtiquetteDto
    {

        public int AgriculteurId { get; set; }

        public int FermeId { get; set; }

        public int ParcelleId { get; set; }

        public int ProduitId { get; set; }

        public int VarieteId { get; set; }

        public string CodeEtiquette { get; set; } = string.Empty;
    }
}
