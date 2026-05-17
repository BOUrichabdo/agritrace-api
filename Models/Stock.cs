using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TracAgriApi.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        // relation reception
        public int ReceptionId { get; set; }

        [ForeignKey("ReceptionId")]
        public Reception? Reception { get; set; }

        // produit
        public int ProduitId { get; set; }

        [ForeignKey("ProduitId")]
        public Produit? Produit { get; set; }

        // variété
        public int VarieteId { get; set; }

        [ForeignKey("VarieteId")]
        public Variete? Variete { get; set; }

        // quantité disponible
        public decimal QuantiteDisponible { get; set; }

        // date entrée stock
        public DateTime DateEntree { get; set; }

        // emplacement futur
        public string? Emplacement { get; set; }

        public int SocieteId { get; set; }


        // état stock
        public string EtatStock { get; set; } = "Disponible";
    }
}
