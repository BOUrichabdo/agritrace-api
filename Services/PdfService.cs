
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using TracAgriApi.DTOs;
namespace TracAgriApi.Services
{
    public class PdfService
    {

        public byte[] Generate(EtiquetteFermeDto dto)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(15);

                    page.Content().Column(col =>
                    {
                        // =========================
                        // TITRE
                        // =========================
                        col.Item()
                            .Text("ETIQUETTE PRODUIT")
                            .Bold()
                            .AlignCenter()
                            .FontSize(18);

                        col.Item().PaddingBottom(10);

                        // =========================
                        // LAYOUT HORIZONTAL PRO
                        // =========================
                        col.Item().Row(row =>
                        {
                            // =====================
                            // LEFT : INFOS
                            // =====================
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Text($"Code: {dto.CodeEtiquette}");
                                left.Item().Text($"Agriculteur: {dto.AgriculteurNom}");
                                left.Item().Text($"Ferme: {dto.FermeNom}");
                                left.Item().Text($"Parcelle: {dto.ParcelleNom}");
                                left.Item().Text($"Produit: {dto.ProduitNom}");
                                left.Item().Text($"Variété: {dto.VarieteNom}");
                                left.Item().Text($"Date: {dto.DateGeneration:dd/MM/yyyy}");
                            });

                            // =====================
                            // RIGHT : QR CODE
                            // =====================
                            row.ConstantItem(130).Column(qr =>
                            {
                                qr.Item()
                                    .AlignCenter()
                                    .Width(120)
                                    .Height(120)
                                    .Image(dto.QrCode);
                            });
                        });
                    });
                });
            })
            .GeneratePdf();
        }
        // generer pdf Etiquette fereme
        //public byte[] Generate(EtiquetteFermeDto dto)
        //{
        //    return Document.Create(container =>
        //    {
        //        container.Page(page =>
        //        {
        //            page.Margin(20);

        //            page.Content().Column(col =>
        //            {
        //                col.Item().Text("ETIQUETTE PRODUIT")
        //                    .Bold().FontSize(20);

        //                col.Item().Text($"Code: {dto.CodeEtiquette}");
        //                col.Item().Text($"Agriculteur: {dto.AgriculteurNom}");
        //                col.Item().Text($"Ferme: {dto.FermeNom}");
        //                col.Item().Text($"Parcelle: {dto.ParcelleNom}");
        //                col.Item().Text($"Produit: {dto.ProduitNom}");
        //                col.Item().Text($"Variété: {dto.VarieteNom}");
        //                col.Item().Text($"Date: {dto.DateGeneration}");
        //                col.Item().Text("QR CODE").Bold();

        //                col.Item()
        //                    .PaddingTop(5)
        //                    .AlignCenter()
        //                    .Width(100)
        //                    .Height(100)
        //                    .Image(dto.QrCode);
        //            });
        //        });
        //    })
        //    .GeneratePdf();
        //}
        // genrere pdf palette reception
        public byte[] GeneratePalette(PalettePrintDto dto)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A6);
                    page.Margin(10);

                    page.Content().Column(col =>
                    {
                        // =========================
                        // TITRE CENTRÉ
                        // =========================
                        col.Item()
                            .AlignCenter()
                            .Text("PALETTE RECEPTION")
                            .Bold()
                            .FontSize(18)
                            ;

                        // =========================
                        // SOUS TITRE
                        // =========================
                        //col.Item()
                        //    .AlignCenter()
                        //    .Text("Système de traçabilité agricole")
                        //    .FontSize(10)
                        //    .FontColor("#6B7280")
                        //    ;

                        // =========================
                        // LAYOUT HORIZONTAL (INFOS + QR)
                        // =========================
                        col.Item().Row(row =>
                        {
                            // =====================
                            // LEFT : INFOS
                            // =====================
                            row.RelativeItem().Column(left =>
                            {
                                left.Spacing(3);

                                left.Item().Text($"Code: {dto.CodePalette}");
                                left.Item().Text($"Agriculteur: {dto.AgriculteurNom}");
                                left.Item().Text($"Ferme: {dto.FermeNom}");
                                left.Item().Text($"Produit: {dto.ProduitNom}");
                                left.Item().Text($"Variété: {dto.VarieteNom}");
                                left.Item().Text($"Poids: {dto.PoidsBrut} KG");
                                left.Item().Text($"Date: {dto.DateCreation:dd/MM/yyyy}");
                            });

                            // =====================
                            // RIGHT : QR CODE
                            // =====================
                            row.ConstantItem(120).Column(qr =>
                            {
                                qr.Item()
                                    .AlignCenter()
                                    .PaddingTop(5)
                                    .Width(110)
                                    .Height(110)
                                    .Image(dto.QrCode);
                            });
                        });
                    });
                });
            })
            .GeneratePdf();
        }
    }
}

