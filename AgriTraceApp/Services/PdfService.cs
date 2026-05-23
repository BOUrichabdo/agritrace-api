
using AgriTraceApp.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class PdfService
{
    public byte[] Generate(EtiquetteDto dto)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(20);

                page.Content().Column(col =>
                {
                    col.Item().Text("ÉTIQUETTE PRODUIT")
                        .Bold().FontSize(20);

                    col.Item().Text($"Code : {dto.CodeEtiquette}");
                    col.Item().Text($"Agriculteur : {dto.AgriculteurNom}");
                    col.Item().Text($"Ferme : {dto.FermeNom}");
                    col.Item().Text($"Parcelle : {dto.ParcelleNom}");
                    col.Item().Text($"Produit : {dto.ProduitNom}");
                    col.Item().Text($"Variété : {dto.VarieteNom}");
                    col.Item().Text($"Date : {dto.DateGeneration}");
                });
            });
        })
        .GeneratePdf();
    }
}