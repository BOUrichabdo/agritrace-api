using AgriTraceApp.DTOs;
using AgriTraceApp.Services;
using Android.Widget;
using Java.Util;
using Microsoft.Maui.Graphics.Text;
using System.Threading.Tasks;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace AgriTraceApp;

public partial class Reception : ContentPage
{

    private int selectedMenuIndex = 0; // 0=Accueil, 1=Historique, 2=Actualiser

    private bool _isScanning = false;

    bool _isDrawerOpen = false;
    double _startY;
    private bool menuOpen = false;


    private int _paletteId;
    private readonly ReceptionService _service = new();
    private readonly EtiquetteFermeService _serviceetiquetteferme = new();

    private EtiquetteDto? _etiquette;
    // declare les var globale
    string? etiquetteId;
    string? poids;
    string temperature;
    string etat;
    string type;

    string observation;
    public Reception()
    {
        InitializeComponent();

        //SetSelectedMenu(0); // Accueil sélectionné par défaut



    }

    // reset formulaire  actualiser form 
    private async Task ResetForm()
    {
        // =========================
        // RESET ENTRY
        // =========================

        txtQRCode.Text = string.Empty;

        TXT_POIDS.Text = string.Empty;

        TXT_TEMPERATURE.Text = string.Empty;

        TXT_OBSERVATION.Text = string.Empty;

        // =========================
        // RESET PICKERS
        // =========================

        CMBETAT.SelectedItem = null;

        CMBTYPE.SelectedItem = null;

        // =========================
        // RESET LABELS
        // =========================

        lblagriculteur.Text = string.Empty;

        lblferme.Text = string.Empty;

        lblproduit.Text = string.Empty;

        lblvariete.Text = string.Empty;

        // =========================
        // RESET VARIABLES
        // =========================

        etiquetteId = null;

        _etiquette = null;

        // =========================
        // RESET CAMERA
        // =========================

        CameraContainer.IsVisible = false;

        CameraContainer.Children.Clear();

        _isScanning = false;

        // =========================
        // RESET BUTTON
        // =========================

        btnScan.Text = "📷 SCANNER QR CODE";

        btnScan.IsEnabled = true;

        // =========================
        // RESET ERRORS
        // =========================

        PoidsError.IsVisible = false;

        TemperatureError.IsVisible = false;

        EtatError.IsVisible = false;

        TypeError.IsVisible = false;


        ValiderReception.IsVisible = true;

        BtnImprimer.IsVisible = false;

        // =========================
        // SCROLL TOP
        // =========================

        await MainScroll.ScrollToAsync(
            0,
            0,
            true);
    }








    private bool ValidateForm()
    {
        bool isValid = true;

        // RESET erreurs
        PoidsError.IsVisible = false;
        TemperatureError.IsVisible = false;

        // 1. POIDS OBLIGATOIRE
        if (string.IsNullOrWhiteSpace(TXT_POIDS.Text))
        {
            PoidsError.Text = "Le poids est obligatoire";
            PoidsError.IsVisible = true;
            isValid = false;
        }

        // 2. TEMPERATURE OBLIGATOIRE
        if (string.IsNullOrWhiteSpace(TXT_TEMPERATURE.Text))
        {
            TemperatureError.Text = "La température est obligatoire";
            TemperatureError.IsVisible = true;
            isValid = false;
        }

        return isValid;
    }




    //Validation reception 
    private async void ValiderReception_Clicked(object sender, EventArgs e)
    {
        // controle saisi 


        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        if (!ValidateForm())
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            return;


        }

           



        string recap =
     $"📦 Réception\n\n" +
     $"🏷️ Etiquette : {etiquetteId}\n" +
     $"⚖️ Poids brut : {TXT_POIDS.Text} KG\n" +
     $"🌡️ Température : {TXT_TEMPERATURE.Text} °C\n" +
     $"📋 État : {(CMBETAT.SelectedItem?.ToString() ?? "Non renseigné")}\n" +
     $"🏭 Type : {(CMBTYPE.SelectedItem?.ToString() ?? "Non renseigné")}\n\n" +
     $"Confirmez-vous cette réception ?";

        bool confirmer = await DisplayAlert(
            "Validation Réception",
            recap,
            "Confirmer",
            "Annuler");

        if (!confirmer)
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            return;
        }












        // Vérifier que etiquetteId est un nombre valide
        if (!int.TryParse(etiquetteId, out int etiquetteIdInt))
        {
            await DisplayAlert("Erreur", "ID etiquette invalide", "OK");
            return;
        }
        bool isValid = true;
        // reset erreurs
        PoidsError.IsVisible = false;
        TemperatureError.IsVisible = false;
        EtatError.IsVisible = false;
        TypeError.IsVisible = false;
        // =========================
        // POIDS
        // =========================
        if (string.IsNullOrWhiteSpace(TXT_POIDS.Text))
        {
            PoidsError.Text = "Poids obligatoire";
            PoidsError.IsVisible = true;
            isValid = false;
        }

        // =========================
        // TEMPERATURE
        // =========================

        if (string.IsNullOrWhiteSpace(TXT_TEMPERATURE.Text))
        {
            TemperatureError.Text = "Température obligatoire";
            TemperatureError.IsVisible = true;
            isValid = false;
        }

        // =========================
        // ETAT
        // =========================

        if (CMBETAT.SelectedItem == null)
        {
            EtatError.Text = "Choisir état produit";
            EtatError.IsVisible = true;
            isValid = false;
        }

        // =========================
        // TYPE
        // =========================

        if (CMBTYPE.SelectedItem == null)
        {
            TypeError.Text = "Choisir type produit";
            TypeError.IsVisible = true;
            isValid = false;
        }

        // stop si erreur
        if (!isValid)
            return;

        try
        {

            // creation reception 
            var dto = new CreateReceptionDto
            {
                EtiquetteFermeId = int.Parse(etiquetteId),
                PoidsBrut = decimal.Parse(TXT_POIDS.Text),
                Temperature = decimal.Parse(TXT_TEMPERATURE.Text),
                EtatProduit = CMBETAT.SelectedItem.ToString(),
                TypeProduit = CMBTYPE.SelectedItem.ToString(),
                Observation = TXT_OBSERVATION.Text ?? "",
                Utilisateur = "admin",
                SocieteId = 1 // ✅ AJOUTER SocieteId
            };
            // consomation API create reception 
            var result =
                await _service.CreateReception(dto);








            await DisplayAlert(
                "Succès",
                $"Palette créée : {result.CodePalette}",
                "OK");






            BtnImprimer.IsVisible = true;

            ValiderReception.IsVisible = false;

            // instance reception recetion service
            var service = new ReceptionService();

            //// DEBUG (optionnel)
            //await DisplayAlert("DEBUG", $"ID = {_etiquette.Id}", "OK");

            // =========================
            // 3. APPEL API PDF
            // =========================
            //var pdfBytes = await service.PrintPalette(result.palette);

            var pdfBytes = await service.PrintPalette(result.PaletteId);

            _paletteId = result.PaletteId;

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                await DisplayAlert("Erreur", "PDF vide ou introuvable", "OK");
                return;
            }

            // =========================
            // 4. SAUVEGARDE LOCAL  CodeEtiquette
            // =========================
            string fileName = $"PAL_{_etiquette.Id}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            File.WriteAllBytes(filePath, pdfBytes);

            // =========================
            // 5. OUVERTURE PDF
            // =========================
            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                Title = "Palette PDF",
                File = new ReadOnlyFile(filePath)
            });



        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Erreur",
                ex.Message,
                "OK");
        }
        finally
        {


            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }














        // controle saisi de formulaire reception





        //try
        //{
        //    var dto = new CreateReceptionDto
        //    {
        //        EtiquetteFermeId = int.Parse(etiquetteId),
        //        PoidsBrut = decimal.Parse(poids),
        //        Temperature = decimal.Parse(temperature),
        //        EtatProduit = etat,
        //        TypeProduit = type,
        //        Observation = observation,
        //        Utilisateur = "admin"
        //    };

        //    var result = await _service.CreateReception(dto);

        //    await DisplayAlert(
        //        "Succès",
        //        $"Palette créée : {result.CodePalette}",
        //        "OK");
        //}
        //catch (Exception ex)
        //{
        //    await DisplayAlert("Erreur", ex.Message, "OK");
        //}



    }

    // remplire info etiq
    private async Task LoadEtiquette(string code)
    {
        try
        {
            var etiquette =
                await _serviceetiquetteferme
                .GetEtiquetteByCode(code);

            if (etiquette == null)
            {
                await DisplayAlert(
                    "Erreur",
                    "Etiquette introuvable",
                    "OK");

                return;
            }

            // sauvegarder objet
            _etiquette = etiquette;

            // IMPORTANT
            etiquetteId = etiquette.Id.ToString();

            // AFFICHAGE UI
            lblagriculteur.Text =
                etiquette.AgriculteurNom;

            lblferme.Text =
                etiquette.FermeNom;

            lblproduit.Text =
                etiquette.ProduitNom;

            lblvariete.Text =
                etiquette.VarieteNom;

            // afficher code QR dans entry
            txtQRCode.Text =
                etiquette.CodeEtiquette;
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Erreur",
                ex.Message,
                "OK");
        }
    }
    // creation camera 
    private void RecreateCameraView()
    {
        try
        {
            // Créer une nouvelle instance de la caméra
            var newCameraView = new CameraBarcodeReaderView()
            {
                CameraLocation = CameraLocation.Rear,
                IsDetecting = true,
                //VerticalOptions = LayoutOptions.FillAndExpand,

                VerticalOptions = LayoutOptions.Fill,

                HorizontalOptions = LayoutOptions.Fill
            };
            newCameraView.BarcodesDetected += NewCameraView_BarcodesDetected1; 

            // Vider le conteneur et ajouter la nouvelle caméra
            CameraContainer.Children.Clear();
            CameraContainer.Children.Add(newCameraView);

            // Mettre à jour la référence
            cameraView = newCameraView;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur création caméra: {ex.Message}");
        }
    }

    private async void NewCameraView_BarcodesDetected1(object? sender, BarcodeDetectionEventArgs e)
    {
        if (!_isScanning)
            return;

        var result = e.Results?.FirstOrDefault();

        if (result == null || string.IsNullOrEmpty(result.Value))
            return;

        // Arrêter le scan
        _isScanning = false;

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {

            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            try
            {
                // Désactiver la détection
                cameraView.IsDetecting = false;

                // AFFICHER LE QR CODE DANS L'ENTRY
                string qrCode = result.Value;
                txtQRCode.Text = qrCode;

                // Optionnel : Afficher une alerte en plus
                //await DisplayAlert("QR Code Détecté", $"Code: {qrCode}", "OK");
                // Cacher le conteneur de la caméra
                CameraContainer.IsVisible = false;
                // Vider le conteneur pour libérer les ressources
                CameraContainer.Children.Clear();
                // Réinitialiser le bouton
                btnScan.Text = "📷 SCANNER QR CODE";
                btnScan.IsEnabled = true;
                // controle de text qr code 
                if (string.IsNullOrWhiteSpace(txtQRCode.Text))
                {
                    await DisplayAlert("Erreur", "Veuillez scanner un QR code", "OK");
                    return;
                }
                await LoadEtiquette(txtQRCode.Text.Trim());

                // Optionnel : Vous pouvez maintenant utiliser ce QR code
                // await TraiterQRCode(qrCode);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", ex.Message, "OK");
            }
            finally
            {

                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        });
    }

    // detected qr code 
    private async void NewCameraView_BarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
    {
        if (!_isScanning)
            return;

        var result = e.Results?.FirstOrDefault();

        if (result == null || string.IsNullOrEmpty(result.Value))
            return;

        // Arrêter le scan
        _isScanning = false;

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {

            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            try
            {
                // Désactiver la détection
                cameraView.IsDetecting = false;

                // AFFICHER LE QR CODE DANS L'ENTRY
                string qrCode = result.Value;
                txtQRCode.Text = qrCode;

                // Optionnel : Afficher une alerte en plus
                //await DisplayAlert("QR Code Détecté", $"Code: {qrCode}", "OK");

                // Cacher le conteneur de la caméra
                CameraContainer.IsVisible = false;

                // Vider le conteneur pour libérer les ressources
                CameraContainer.Children.Clear();

                // Réinitialiser le bouton
                btnScan.Text = "📷 SCANNER QR CODE";
                btnScan.IsEnabled = true;



                // controle de text qr code 
                if (string.IsNullOrWhiteSpace(txtQRCode.Text))
                {
                    await DisplayAlert("Erreur", "Veuillez scanner un QR code", "OK");
                    return;
                }


                await LoadEtiquette(txtQRCode.Text.Trim());

                // Optionnel : Vous pouvez maintenant utiliser ce QR code
                // await TraiterQRCode(qrCode);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", ex.Message, "OK");
            }
            finally
            {

                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        });
    }
    // Afficher camerre 
    private async void btnScan_Clicked(object sender, EventArgs e)
    {

        try
        {
            // Demander permission caméra
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Erreur", "Permission caméra refusée", "OK");
                return;
            }

            // Vider le champ Entry avant le scan
            txtQRCode.Text = string.Empty;

            // IMPORTANT: Réinitialiser complètement le conteneur caméra
            CameraContainer.Children.Clear();

            // Créer une NOUVELLE instance de caméra
            var newCameraView = new CameraBarcodeReaderView
            {
                CameraLocation = CameraLocation.Rear,
                IsDetecting = true,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            // Ajouter le gestionnaire d'événement (cameraView_BarcodesDetected_1)
            newCameraView.BarcodesDetected += cameraView_BarcodesDetected_1;

            // Ajouter au conteneur
            CameraContainer.Children.Add(newCameraView);

            // Mettre à jour la référence (important pour que le XAML fonctionne)
            cameraView = newCameraView;

            // Afficher le conteneur
            CameraContainer.IsVisible = true;

            // Attendre un peu pour que la caméra s'initialise
            await Task.Delay(500);

            // Activer le flag de scan
            _isScanning = true;

            // Désactiver le bouton pendant le scan
            btnScan.Text = "🔍 SCAN EN COURS...";
            btnScan.IsEnabled = false;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Erreur: {ex.Message}", "OK");
            btnScan.Text = "📷 SCANNER QR CODE";
            btnScan.IsEnabled = true;
        }
        //try
        //{
        //    // Demander permission caméra
        //    var status = await Permissions.RequestAsync<Permissions.Camera>();

        //    if (status != PermissionStatus.Granted)
        //    {
        //        await DisplayAlert("Erreur", "Permission caméra refusée", "OK");
        //        return;
        //    }

        //    // Vider le champ Entry avant le scan
        //    txtQRCode.Text = string.Empty;

        //    // Afficher le conteneur de la caméra
        //    CameraContainer.IsVisible = true;

        //    // Attendre un peu que l'UI se mette à jour
        //    await Task.Delay(100);

        //    // Recréer la caméra pour éviter l'écran noir
        //    RecreateCameraView();

        //    // Activer la détection
        //    _isScanning = true;
        //    cameraView.IsDetecting = true;

        //    // Désactiver le bouton pendant le scan
        //    btnScan.Text = "🔍 SCAN EN COURS...";
        //    btnScan.IsEnabled = false;
        //}
        //catch (Exception ex)
        //{
        //    await DisplayAlert("Erreur", ex.Message, "OK");
        //}

    }
    // afficher information 
    private async void Sqcannebtn_Clicked(object sender, EventArgs e)
    {

        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        try
        {

            // controle de text qr code 
            if (string.IsNullOrWhiteSpace(txtQRCode.Text))
            {
                await DisplayAlert("Erreur", "Veuillez scanner un QR code", "OK");
                return;
            }


            await LoadEtiquette(txtQRCode.Text.Trim());


        }
        catch (Exception ex)
        {

            await DisplayAlert(
               "Erreur",
               ex.Message,
               "OK");


        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;


        }


    }

    private async void CLOSECAMERE_Clicked(object sender, EventArgs e)
    {

        try
        {
            _isScanning = false;

            if (cameraView != null)
            {
                cameraView.IsDetecting = false;
            }

            // cacher camera
            CameraContainer.IsVisible = false;

            // vider container
            CameraContainer.Children.Clear();

            // reset bouton
            btnScan.Text = "📷 SCANNER QR CODE";
            btnScan.IsEnabled = true;

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Erreur",
                ex.Message,
                "OK");
        }

    }

    private void TXT_OBSERVATION_TextChanged(object sender, TextChangedEventArgs e)
    {

        if (string.IsNullOrWhiteSpace(TXT_OBSERVATION.Text))
            return;

        string text = TXT_OBSERVATION.Text;

        // première lettre majuscule
        string formatted =
            char.ToUpper(text[0]) + text.Substring(1);

        // éviter boucle infinie
        if (TXT_OBSERVATION.Text != formatted)
        {
            TXT_OBSERVATION.Text = formatted;
        }

    }

    private async Task LoadHistory()
    {
        //try
        //{
        //    var data = await _service.CreateReception(); // ton API

        //    HistoryList.ItemsSource = data;
        //}
        //catch (Exception ex)
        //{
        //    await DisplayAlert("Erreur", ex.Message, "OK");
        //}
    }

    //private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    //{

    //    HistoryDrawer.IsVisible = !HistoryDrawer.IsVisible;

    //    if (HistoryDrawer.IsVisible)
    //    {
    //        HistoryDrawer.TranslationY = -50;
    //        HistoryDrawer.Opacity = 0;

    //        await Task.WhenAll(
    //            HistoryDrawer.FadeTo(1, 250),
    //            HistoryDrawer.TranslateTo(0, 0, 250, Easing.CubicOut)
    //        );

    //        await LoadHistory();
    //    }

    //}

    private void BtnImprimer_Clicked(object sender, EventArgs e)
    {

    }

    private async void BtnImprimer_Clicked_1(object sender, EventArgs e)
    {

        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        if (!ValidateForm())

        {

            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            return;



        }



        string recap =
     $"📦 Réception\n\n" +
     $"🏷️ Etiquette : {etiquetteId}\n" +
     $"⚖️ Poids brut : {TXT_POIDS.Text} KG\n" +
     $"🌡️ Température : {TXT_TEMPERATURE.Text} °C\n" +
     $"📋 État : {(CMBETAT.SelectedItem?.ToString() ?? "Non renseigné")}\n" +
     $"🏭 Type : {(CMBTYPE.SelectedItem?.ToString() ?? "Non renseigné")}\n\n" +
     $"Confirmez-vous cette réception ?";

        bool confirmer = await DisplayAlert(
            "Validation Réception",
            recap,
            "Confirmer",
            "Annuler");


        if (!confirmer)
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            return;
        }

        // Vérifier que etiquetteId est un nombre valide
        if (!int.TryParse(etiquetteId, out int etiquetteIdInt))
        {
            await DisplayAlert("Erreur", "ID etiquette invalide", "OK");
            return;
        }
        bool isValid = true;
        // reset erreurs
        PoidsError.IsVisible = false;
        TemperatureError.IsVisible = false;
        EtatError.IsVisible = false;
        TypeError.IsVisible = false;
        // =========================
        // POIDS
        // =========================
        if (string.IsNullOrWhiteSpace(TXT_POIDS.Text))
        {
            PoidsError.Text = "Poids obligatoire";
            PoidsError.IsVisible = true;
            isValid = false;
        }

        // =========================
        // TEMPERATURE
        // =========================

        if (string.IsNullOrWhiteSpace(TXT_TEMPERATURE.Text))
        {
            TemperatureError.Text = "Température obligatoire";
            TemperatureError.IsVisible = true;
            isValid = false;
        }

        // =========================
        // ETAT
        // =========================

        if (CMBETAT.SelectedItem == null)
        {
            EtatError.Text = "Choisir état produit";
            EtatError.IsVisible = true;
            isValid = false;
        }

        // =========================
        // TYPE
        // =========================

        if (CMBTYPE.SelectedItem == null)
        {
            TypeError.Text = "Choisir type produit";
            TypeError.IsVisible = true;
            isValid = false;
        }

        // stop si erreur
        if (!isValid)
            return;










        try
        {





            if (_paletteId <= 0)
            {
                await DisplayAlert("Erreur", "Aucune palette à imprimer", "OK");
                return;
            }

            var service = new ReceptionService();

            var pdfBytes = await service.PrintPalette(_paletteId);

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                await DisplayAlert("Erreur", "PDF vide ou introuvable", "OK");
                return;
            }

            string fileName =
                $"PAL_{_paletteId}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";

            string filePath =
                Path.Combine(FileSystem.CacheDirectory, fileName);

            File.WriteAllBytes(filePath, pdfBytes);

            await Launcher.Default.OpenAsync(
                new OpenFileRequest
                {
                    Title = "Palette PDF",
                    File = new ReadOnlyFile(filePath)
                });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }

    }



















    private async void BTNACTUALISER_Clicked_1(object sender, EventArgs e)
    {
        //DrawerOverlay.IsVisible = true;
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        try
        {

            await ResetForm();

            ValiderReception.IsVisible = true;

            //BtnImprimer.IsVisible = false;



        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger les réceptions: {ex.Message}", "OK");



        }
        finally
        {

            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;




        }



















        //// reset scan
        //txtQRCode.Text = "";

        //// reset champs
        //TXT_POIDS.Text = "";
        //TXT_TEMPERATURE.Text = "";

        //CMBETAT.SelectedItem = null;
        //CMBTYPE.SelectedItem = null;

        //TXT_OBSERVATION.Text = "";

        //// reset variables
        //_paletteId = 0;
        //_etiquette = null;

        //// cacher bouton imprimer
        //BtnImprimer.IsVisible = false;

        //await DisplayAlert("Reset", "Formulaire réinitialisé", "OK");

    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {

        //SetSelectedMenu(0);


    }

    // historique 
    private async void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {

        //SetSelectedMenu(1);

        await Navigation.PushAsync(new Historique());


    }


    // actualiser page 
    private async Task TapGestureRecognizer_Tapped_2(object sender, TappedEventArgs e)
    {
        ///*SetSelectedMenu*/(2);
        //RefreshData();

        await ResetForm();



    }

    //private void SetSelectedMenu(int index)
    //{
    //    selectedMenuIndex = index;

    //    // Réinitialiser tous les styles
    //    ResetMenuStyle(MenuAcceuil, IconAcceuil, LblAcceuil);
    //    ResetMenuStyle(MenuHistorique, IconHistorique, LblHistorique);
    //    ResetMenuStyle(MenuActualiser, IconActualiser, LblActualiser);

    //    // Appliquer le style sélectionné
    //    switch (index)
    //    {
    //        case 0:
    //            SetActiveMenuStyle(MenuAcceuil, IconAcceuil, LblAcceuil);
    //            break;
    //        case 1:
    //            SetActiveMenuStyle(MenuHistorique, IconHistorique, LblHistorique);
    //            break;
    //        case 2:
    //            SetActiveMenuStyle(MenuActualiser, IconActualiser, LblActualiser);
    //            break;
    //    }
    //}
    private void ResetMenuStyle(Grid menu, Label icon, Label text)
    {
        if (menu != null)
            menu.BackgroundColor = Colors.Transparent;

        if (icon != null)
            icon.TextColor = Color.FromArgb("#9CA3AF"); // Gris

        if (text != null)
            text.TextColor = Color.FromArgb("#9CA3AF"); // Gris
    }

    private void SetActiveMenuStyle(Grid menu, Label icon, Label text)
    {
        if (menu != null)
            menu.BackgroundColor = Color.FromArgb("#EFF6FF"); // Bleu très clair

        if (icon != null)
            icon.TextColor = Color.FromArgb("#2196F3"); // Bleu principal

        if (text != null)
            text.TextColor = Color.FromArgb("#2196F3"); // Bleu principal
    }

    private async void RefreshData()
    {
        // Afficher un indicateur de chargement
        //await DisplayAlert("Actualisation", "Données actualisées avec succès", "OK");

        // Votre logique d'actualisation ici
        // Recharger les données, vider les champs, etc.

        // Remettre le focus sur Accueil après 1 seconde
        await Task.Delay(1000);
        //SetSelectedMenu(0);
    }

    private async void TapGestureRecognizer_Tapped_3(object sender, TappedEventArgs e)
    {

        //SetSelectedMenu(2);
        //RefreshData();

        await ResetForm();

    }


    private async void cameraView_BarcodesDetected_1(object sender, BarcodeDetectionEventArgs e)
    {

        // Vérifier si on est en mode scan
        if (!_isScanning)
            return;

        // Récupérer le premier code détecté
        var result = e.Results?.FirstOrDefault();
        if (result == null || string.IsNullOrEmpty(result.Value))
            return;

        // Désactiver immédiatement le scan pour éviter les doubles détections
        _isScanning = false;

        // Exécuter sur le thread UI
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                // Afficher le loading
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                // Désactiver la détection de la caméra
                cameraView.IsDetecting = false;

                // Récupérer le code QR
                string qrCode = result.Value;
                txtQRCode.Text = qrCode;

                // Cacher la caméra
                CameraContainer.IsVisible = false;

                // Réinitialiser le bouton scan
                btnScan.Text = "📷 SCANNER QR CODE";
                btnScan.IsEnabled = true;

                // Charger les informations de l'étiquette
                if (!string.IsNullOrWhiteSpace(txtQRCode.Text))
                {
                    await LoadEtiquette(txtQRCode.Text.Trim());
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Erreur lors du scan: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        });



    }
}
