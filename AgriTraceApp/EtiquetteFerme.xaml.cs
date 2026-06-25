using AgriTraceApp.DTOs;
using AgriTraceApp.Models;
using AgriTraceApp.Services;
using SkiaSharp;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using static System.Net.Mime.MediaTypeNames;

namespace AgriTraceApp;

public partial class EtiquetteFerme : ContentPage
{
    // service PDF
    private readonly PdfService _pdfService = new PdfService();
    // service API
    private readonly AgriculteurService _serviceagriculteur = new AgriculteurService();
    //private ApiService _service = new ApiService();
    private readonly ProduitService _serviceproduit = new ProduitService();
    private readonly ParcelleService _serviceparcelle = new ParcelleService();
    private readonly EtiquetteFermeService _serviceetiquetteferme = new EtiquetteFermeService();
    // objet Model agriculteur 
    private List<AgriculteurModel>? _agriculteurs;
    private List<FermeModele> _fermes = new();
    private List<ModeleVariete> _variete = new();
    private List<ParcelleModel> _parcelle = new();
    private List<ProduitModel> _produit = new();
    // etiquette fereme 
    private EtiquetteDto? _etiquette;
    // code etiquette 
    private string qrText = string.Empty;
    public EtiquetteFerme()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadData();


    }
    // remplire information agriculteur dans le combo box
    private async Task LoadData()
    {
        // 🔥 stocker données dans la liste agri
        //_agriculteurs = await _serviceagriculteur.GetAgriculteurs();
        //// 🔥 afficher
        //CMBAGRICULTEUR.ItemsSource = _agriculteurs;
    }
    // remplire parcelle a partir fereme 
    private async void CMBFERME_SelectedIndexChanged_2(object sender, EventArgs e)
    {
        var ferme = CMBFERME.SelectedItem as FermeModele;
        if (ferme == null) return;
        LBL_FERME.Text = $"🏡 Ferme : {ferme?.NomFerme}";
        _parcelle = await _serviceparcelle.GetParcelleByFerme(ferme.Id);
        CMBPARCELLE.ItemsSource = _parcelle;
        CMBPARCELLE.SelectedItem = null;
    }

    private async void CMBPARCELLE_SelectedIndexChanged_1(object sender, EventArgs e)
    {
        var _parcelle = CMBPARCELLE.SelectedItem as ParcelleModel;
        if (_parcelle == null) return;
        LBL_PARCELLE.Text = $"📍 Parcelle : {_parcelle.NomParcelle}";
        _produit = await _serviceproduit.GetProduitByParcelle(_parcelle.Id);
        CMBPRODUIT.ItemsSource = _produit;
        CMBPRODUIT.SelectedItem = null;
    }
    // remplire les ferem aprtir agriculteur
    private async void CMBAGRICULTEUR_SelectedIndexChanged_2(object sender, EventArgs e)
    {
        var agri = CMBAGRICULTEUR.SelectedItem as AgriculteurModel;
        LBL_AGRI.Text = $"👨‍🌾 Agriculteur : {agri?.Nom}";
        if (agri == null) return;
        _fermes = await _serviceagriculteur.GetFermesByAgriculteur(agri.Id);
        CMBFERME.ItemsSource = _fermes;
        CMBFERME.SelectedItem = null;
        CMBPARCELLE.ItemsSource = null;

    }

    private async void CMBPRODUIT_SelectedIndexChanged_1(object sender, EventArgs e)
    {
        // recup info selectionné produit
        var produit = CMBPRODUIT.SelectedItem as ProduitModel;
        // verification info produit
        if (produit == null)
            return;
        //Q afficher inf produit dans apercu
        LBL_PRODUIT.Text = $"📦 Produit : {produit.Nom}";
        // 🔢 code produit
        //LBL_CODE.Text = $"🔢 Code : PRD-{produit.Id}";
        // recup variete du produit
        var variete = await _serviceproduit.GetVarieteByProduit(produit.VarieteId);
        // verifier info variete
        if (variete != null)
        {
            CMBVARIETE.ItemsSource = new List<ModeleVariete>
        {
            variete
        };

            CMBVARIETE.SelectedItem = variete;

            LBL_VARIETE.Text = $"🌱 Variété : {variete.Intitule}";

        }
    }
    // btn imprimer 
    private async void Imprimer_Clicked_1(object sender, EventArgs e)
    {

        try
        {
            // =========================
            // 1. VALIDATION
            // =========================
            if (_etiquette == null || _etiquette.Id <= 0)
            {
                await DisplayAlert("Erreur", "Aucune étiquette sélectionnée", "OK");
                return;
            }

            // =========================
            // 2. SERVICE
            // =========================
            var service = new EtiquetteFermeService();

            //// DEBUG (optionnel)
            //await DisplayAlert("DEBUG", $"ID = {_etiquette.Id}", "OK");

            // =========================
            // 3. APPEL API PDF
            // =========================
            var pdfBytes = await service.GetPdf(_etiquette.Id);

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                await DisplayAlert("Erreur", "PDF vide ou introuvable", "OK");
                return;
            }

            // =========================
            // 4. SAUVEGARDE LOCAL
            // =========================
            string fileName = $"etiquette_{_etiquette.Id}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            File.WriteAllBytes(filePath, pdfBytes);

            // =========================
            // 5. OUVERTURE PDF
            // =========================
            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                Title = "Étiquette PDF",
                File = new ReadOnlyFile(filePath)
            });
        }
        catch (HttpRequestException httpEx)
        {
            await DisplayAlert("Erreur Réseau", httpEx.Message, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }

    }
    private byte[] GenerateQrBytes(string text)
    {
        var writer = new ZXing.BarcodeWriterPixelData
        {
            Format = ZXing.BarcodeFormat.QR_CODE,
            Options = new ZXing.Common.EncodingOptions
            {
                Width = 300,
                Height = 300,
                Margin = 1
            }
        };

        var pixelData = writer.Write(text);

        using var bitmap = new SkiaSharp.SKBitmap(pixelData.Width, pixelData.Height);

        for (int y = 0; y < pixelData.Height; y++)
        {
            for (int x = 0; x < pixelData.Width; x++)
            {
                int i = (y * pixelData.Width + x) * 4;

                bitmap.SetPixel(x, y,
                    new SkiaSharp.SKColor(
                        pixelData.Pixels[i + 2],
                        pixelData.Pixels[i + 1],
                        pixelData.Pixels[i],
                        pixelData.Pixels[i + 3]));
            }
        }
        using var image = SkiaSharp.SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);

        return data.ToArray();
    }
    //public byte[] GenerateQr(string text)
    //{
    //    var writer = new BarcodeWriterPixelData
    //    {
    //        Format = BarcodeFormat.QR_CODE,
    //        Options = new EncodingOptions
    //        {
    //            Width = 300,
    //            Height = 300,
    //            Margin = 1
    //        }
    //    };

    //    var pixelData = writer.Write(text);

    //    using var bitmap = new SkiaSharp.SKBitmap(pixelData.Width, pixelData.Height);

    //    for (int y = 0; y < pixelData.Height; y++)
    //    {
    //        for (int x = 0; x < pixelData.Width; x++)
    //        {
    //            int i = (y * pixelData.Width + x) * 4;

    //            bitmap.SetPixel(x, y,
    //                new SkiaSharp.SKColor(
    //                    pixelData.Pixels[i + 2],
    //                    pixelData.Pixels[i + 1],
    //                    pixelData.Pixels[i],
    //                    pixelData.Pixels[i + 3]));
    //        }
    //    }

    //    using var image = SkiaSharp.SKImage.FromBitmap(bitmap);
    //    using var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);

    //    return data.ToArray();
    //}
    private void GenerateQr(string text)
    {
        var writer = new ZXing.BarcodeWriterPixelData
        {
            Format = ZXing.BarcodeFormat.QR_CODE,
            Options = new ZXing.Common.EncodingOptions
            {
                Width = 300,
                Height = 300,
                Margin = 1
            }
        };

        var pixelData = writer.Write(text);

        using var bitmap = new SkiaSharp.SKBitmap(pixelData.Width, pixelData.Height);

        for (int y = 0; y < pixelData.Height; y++)
        {
            for (int x = 0; x < pixelData.Width; x++)
            {
                int i = (y * pixelData.Width + x) * 4;

                bitmap.SetPixel(x, y,
                    new SkiaSharp.SKColor(
                        pixelData.Pixels[i + 2],
                        pixelData.Pixels[i + 1],
                        pixelData.Pixels[i],
                        pixelData.Pixels[i + 3]));
            }
        }

        using var image = SkiaSharp.SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);

        var stream = new MemoryStream(data.ToArray());

        IMG_QR.Source = ImageSource.FromStream(() => new MemoryStream(stream.ToArray()));
    }
    // send wtsap 
    private async void BtnWhats_Clicked(object sender, EventArgs e)
    {


        // ======================
        // RECUPERATION
        // ======================

        var agriculteur =
            CMBAGRICULTEUR.SelectedItem as AgriculteurModel;

        var produit =
            CMBPRODUIT.SelectedItem as ProduitModel;

        if (agriculteur == null || produit == null)
        {
            await DisplayAlert(
                "Erreur",
                "Sélectionnez agriculteur et produit",
                "OK");

            return;
        }

        // ======================
        // TELEPHONE
        // ======================

        string phone = agriculteur.Telephone;

        if (string.IsNullOrWhiteSpace(phone))
        {
            await DisplayAlert(
                "Erreur",
                "Téléphone introuvable",
                "OK");

            return;
        }

        // ======================
        // QR UNIQUE
        // ======================

        //qrText =
        //   $"ETQ-{produit.Id}-{DateTime.Now:yyyyMMddHHmmss}";

        // ======================
        // LIEN QR API
        // ======================




        string qrUrl = $"https://agritrace-api-production.up.railway.app/api/Qr/{qrText}";


        // ======================
        // MESSAGE WHATSAPP
        // ======================

        string message =
            $"🌾 *Étiquette Produit*\n" +
            $"----------------------\n" +
            $"👨‍🌾 {agriculteur.Nom}\n" +
            $"📦 {produit.Nom}\n" +
            $"🔢 Code : {qrText}\n\n" +
            $"📲 QR Code :\n{qrUrl}\n\n" +
            $"👉 Cliquez sur le lien pour afficher le QR";

        // ======================
        // URL WHATSAPP
        // ======================

        string url =
            $"https://wa.me/{phone}?text={Uri.EscapeDataString(message)}";

        // ======================
        // OUVRIR WHATSAPP
        // ======================

        await Launcher.OpenAsync(url);

    }

    // btn generer etiquette 
    private async void BTN_GENERER_Clicked(object sender, EventArgs e)
    {
        // recup information selectionné
        var agriculteur = CMBAGRICULTEUR.SelectedItem as AgriculteurModel;
        var ferme = CMBFERME.SelectedItem as FermeModele;
        var parcelle = CMBPARCELLE.SelectedItem as ParcelleModel;
        var produit = CMBPRODUIT.SelectedItem as ProduitModel;
        var variete = CMBVARIETE.SelectedItem as ModeleVariete;

        if (agriculteur == null ||
            ferme == null ||
            parcelle == null ||
            produit == null ||
            variete == null)
        {
            await DisplayAlert("Erreur",
                "Veuillez sélectionner toutes les informations",
                "OK");

            return;
        }


        // 🔥 code QR unique creation QR code unique de 
        qrText =
   $"ETQ|{agriculteur.Id}|{ferme.Id}|{parcelle.Id}|{produit.Id}|{DateTime.Now:yyyyMMddHHmmss}";

        // =========================
        // DTO INSERTION create etioquette fereme 
        // =========================

      




        var dto = new CreateEtiquetteDto
        {
            AgriculteurId = agriculteur.Id,
            FermeId = ferme.Id,
            ParcelleId = parcelle.Id,
            ProduitId = produit.Id,
            VarieteId = variete.Id,
            CodeEtiquette = qrText

        };


        // API
        // =========================

        _etiquette =
            await _serviceetiquetteferme.GenererEtiquette(dto);







        if (_etiquette == null)
        {
            await DisplayAlert(
                "Erreur",
                "Erreur API",
                "OK");

            return;
        }
      


        // =========================
        // APERÇU affichage les information etiquatte fereme 
        // =========================

        LBL_AGRI.Text =
            $"👨‍🌾 Agriculteur : {agriculteur.Nom}";

        LBL_FERME.Text =
            $"🏡 Ferme : {ferme.NomFerme}";

        LBL_PARCELLE.Text =
            $"📍 Parcelle : {parcelle.NomParcelle}";

        LBL_PRODUIT.Text =
            $"📦 Produit : {produit.Nom}";

        LBL_VARIETE.Text =
            $"🌱 Variété : {variete.Intitule}";

        LBL_CODE.Text =
            $"🔢 Code : {qrText}";

        // 🔥 QR genrere qr code etquette ferme 
        GenerateQr(qrText);

    }

    private void ImageButton_Clicked(object sender, EventArgs e)
    {

    }

    private async Task ActualiserEtiquette()
    {
        try
        {
            await LoadData(); // recharge agriculteurs, fermes etc.

            if (_etiquette != null)
            {
                //var refreshed = await _serviceetiquetteferme
                //    .GetById(_etiquette.Id);

                //_etiquette = refreshed;
            }

            await DisplayAlert("OK", "Étiquette actualisée", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
    }

    private void ResetForm()
    {
        // Pickers
        CMBAGRICULTEUR.SelectedItem = null;
        CMBFERME.ItemsSource = null;
        CMBPARCELLE.ItemsSource = null;
        CMBPRODUIT.ItemsSource = null;
        CMBVARIETE.ItemsSource = null;

        // Labels
        LBL_AGRI.Text = "👨‍🌾 Agriculteur : -";
        LBL_FERME.Text = "🏡 Ferme : -";
        LBL_PARCELLE.Text = "📍 Parcelle : -";
        LBL_PRODUIT.Text = "📦 Produit : -";
        LBL_VARIETE.Text = "🌱 Variété : -";
        LBL_CODE.Text = "🔢 Code : -";

        // QR
        IMG_QR.Source = null;

        // Model
        _etiquette = null;
    }

    private async void ImageButton_Clicked_1(object sender, EventArgs e)
    {

        string action = await DisplayActionSheet(
       "Actions",
       "Annuler",
       null,
       
       "🧹 Réinitialiser tout"
   );

        switch (action)
        {
            case "🔄 Actualiser étiquette":
                await ActualiserEtiquette();
                break;

            case "🧹 Réinitialiser tout":
                ResetForm();
                break;
        }

    }
}







