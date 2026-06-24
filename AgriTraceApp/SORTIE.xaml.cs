using AgriTraceApp.DTOs;
using AgriTraceApp.Services;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace AgriTraceApp;

public partial class SORTIE : ContentPage
{
    private readonly SortieStockService _service = new();

    private bool _isScanning = false;

    private PaletteSortieDto? _palette;

    public SORTIE()
    {
        InitializeComponent();
    }

    // =========================
    // RESET FORMULAIRE
    // =========================
    private async Task ResetForm()
    {
        // RESET ENTRY
        txtQRCode.Text = string.Empty;
        TXT_QTE_SORTIE.Text = string.Empty;
        TXT_DESTINATION.Text = string.Empty;
        TXT_TRANSPORT.Text = string.Empty;
        TXT_OBSERVATION.Text = string.Empty;

        // RESET LABELS
        LBL_PALETTE.Text = "-";
        LBL_PRODUIT.Text = "-";
        LBL_VARIETE.Text = "-";
        LBL_QTE.Text = "-";
        LBL_ETAT.Text = "-";

        // RESET VARIABLES
        _palette = null;

        // RESET CAMERA
        CameraContainer.IsVisible = false;
        CameraContainer.Children.Clear();
        _isScanning = false;

        // RESET BUTTON
        btnScan.Text = "📷 SCANNER QR CODE";
        btnScan.IsEnabled = true;

        // RESET ERRORS
        QteError.IsVisible = false;
        DestinationError.IsVisible = false;

        // RESET BUTTONS VISIBILITY
        BTNSORTIE.IsVisible = true;
        BtnImprimer.IsVisible = true;

        // SCROLL TOP
        await MainScroll.ScrollToAsync(0, 0, true);
    }


    // =========================
    // CHARGER INFO PALETTE
    // =========================
    private async Task LoadPalette(string code)
    {
        try
        {
            var palette = await _service.GetPaletteByCode(code);

            if (palette == null)
            {
                await DisplayAlert(
                    "Erreur",
                    "Palette introuvable",
                    "OK");
                return;
            }

            // sauvegarder objet
            _palette = palette;

            // AFFICHAGE UI
            LBL_PALETTE.Text = palette.CodePalette;
            LBL_PRODUIT.Text = palette.Produit;
            LBL_VARIETE.Text = palette.Variete;
            LBL_QTE.Text = $"{palette.QuantiteDisponible} KG";
            LBL_ETAT.Text = palette.EtatPalette;

            // afficher code QR dans entry
            txtQRCode.Text = palette.CodePalette;
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Erreur",
                ex.Message,
                "OK");
        }
    }

    // =========================
    // BTN SCANNER QR CODE
    // =========================
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

            // Ajouter le gestionnaire d'événement
            newCameraView.BarcodesDetected += cameraView_BarcodesDetected_1;

            // Ajouter au conteneur
            CameraContainer.Children.Add(newCameraView);

            // Mettre à jour la référence
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
    }

    // =========================
    // DETECTION QR CODE (event caméra)
    // =========================
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

                // Vider le conteneur pour libérer les ressources
                CameraContainer.Children.Clear();

                // Réinitialiser le bouton scan
                btnScan.Text = "📷 SCANNER QR CODE";
                btnScan.IsEnabled = true;

                // Contrôle du code QR
                if (string.IsNullOrWhiteSpace(txtQRCode.Text))
                {
                    await DisplayAlert("Erreur", "Veuillez scanner un QR code", "OK");
                    return;
                }

                // Charger les informations de la palette
                await LoadPalette(txtQRCode.Text.Trim());
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

    // =========================
    // DETECTION QR CODE (event XAML)
    // =========================
    private async void cameraView_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        // Vérifier si on est en mode scan
        if (!_isScanning)
            return;

        // Récupérer le premier code détecté
        var result = e.Results?.FirstOrDefault();
        if (result == null || string.IsNullOrEmpty(result.Value))
            return;

        // Désactiver immédiatement le scan
        _isScanning = false;

        // Exécuter sur le thread UI
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                cameraView.IsDetecting = false;

                string qrCode = result.Value;
                txtQRCode.Text = qrCode;

                CameraContainer.IsVisible = false;
                CameraContainer.Children.Clear();

                btnScan.Text = "📷 SCANNER QR CODE";
                btnScan.IsEnabled = true;

                if (!string.IsNullOrWhiteSpace(txtQRCode.Text))
                {
                    await LoadPalette(txtQRCode.Text.Trim());
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

    // =========================
    // BTN AFFICHER INFORMATION PALETTE
    // =========================
    private async void BtnAfficherInfos_Clicked(object sender, EventArgs e)
    {
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        try
        {
            // Contrôle du code QR
            if (string.IsNullOrWhiteSpace(txtQRCode.Text))
            {
                await DisplayAlert("Erreur", "Veuillez scanner ou saisir un code palette", "OK");
                return;
            }

            await LoadPalette(txtQRCode.Text.Trim());
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

    // =========================
    // BTN VALIDER SORTIE
    // =========================
    private async void BTNSORTIE_Clicked_1(object sender, EventArgs e)
    {
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        try
        {
            // =========================
            // CONTROLES
            // =========================
            bool isValid = true;

            QteError.IsVisible = false;
            DestinationError.IsVisible = false;

            if (_palette == null)
            {
                await DisplayAlert("Erreur", "Veuillez scanner une palette d'abord", "OK");
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(TXT_QTE_SORTIE.Text))
            {
                QteError.Text = "La quantité est obligatoire";
                QteError.IsVisible = true;
                isValid = false;
            }

            if (!isValid)
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                return;
            }

            decimal qte = decimal.Parse(TXT_QTE_SORTIE.Text);

            // Vérifier que la quantité ne dépasse pas la quantité disponible
            if (qte > _palette.QuantiteDisponible)
            {
                QteError.Text = $"Quantité max : {_palette.QuantiteDisponible} KG";
                QteError.IsVisible = true;
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                return;
            }

            // =========================
            // CONFIRMATION
            // =========================
            string recap =
                $"📤 Sortie Stock\n\n" +
                $"🏷️ Palette : {_palette.CodePalette}\n" +
                $"📦 Produit : {_palette.Produit}\n" +
                $"🌱 Variété : {_palette.Variete}\n" +
                $"⚖️ Quantité sortie : {qte} KG\n" +
                $"📍 Destination : {(TXT_DESTINATION.Text ?? "Non renseigné")}\n" +
                $"🚚 Transporteur : {(TXT_TRANSPORT.Text ?? "Non renseigné")}\n\n" +
                $"Confirmez-vous cette sortie ?";

            bool confirmer = await DisplayAlert(
                "Validation Sortie",
                recap,
                "Confirmer",
                "Annuler");

            if (!confirmer)
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                return;
            }

            // =========================
            // DTO
            // =========================
            var dto = new CreateSortieStockDto
            {
                CodePalette = _palette.CodePalette,
                QuantiteSortie = qte,
                Utilisateur = "admin",
                Observation = TXT_OBSERVATION.Text ?? ""
            };

            // =========================
            // API
            // =========================
            var result = await _service.CreateSortie(dto);

            if (result)
            {
                await DisplayAlert(
                    "Succès",
                    $"Sortie stock réussie\nPalette : {_palette.CodePalette}\nQuantité : {qte} KG",
                    "OK");

                // Recharger les infos de la palette mise à jour
                await LoadPalette(_palette.CodePalette);

                // Vider les champs de sortie (pas les infos palette)
                TXT_QTE_SORTIE.Text = string.Empty;
                TXT_DESTINATION.Text = string.Empty;
                TXT_TRANSPORT.Text = string.Empty;
                TXT_OBSERVATION.Text = string.Empty;
            }
            else
            {
                await DisplayAlert(
                    "Erreur",
                    "Erreur lors de la sortie stock",
                    "OK");
            }
        }
        catch (FormatException)
        {
            await DisplayAlert("Erreur", "Quantité invalide", "OK");
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

    // =========================
    // BTN IMPRIMER
    // =========================
    private async void BtnImprimer_Clicked(object sender, EventArgs e)
    {
        if (_palette == null)
        {
            await DisplayAlert("Erreur", "Aucune palette sélectionnée", "OK");
            return;
        }

        // Afficher récapitulatif palette
        string info =
            $"🏷️ Palette : {_palette.CodePalette}\n" +
            $"📦 Produit : {_palette.Produit}\n" +
            $"🌱 Variété : {_palette.Variete}\n" +
            $"⚖️ Qté disponible : {_palette.QuantiteDisponible} KG\n" +
            $"📋 État : {_palette.EtatPalette}";

        await DisplayAlert("Informations Palette", info, "OK");
    }

    // =========================
    // NAVIGATION - RECEPTION
    // =========================
    private async void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new Reception());
    }

    // =========================
    // NAVIGATION - HISTORIQUE
    // =========================
    private async void TapGestureRecognizer_Tapped_2(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new Historique());
    }

    // =========================
    // ACTUALISER
    // =========================
    private async void TapGestureRecognizer_Tapped_3(object sender, TappedEventArgs e)
    {
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        try
        {
            await ResetForm();
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

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
    }

    // =========================
    // ANCIEN BOUTON SCANNER PALETTE (conservé pour compatibilité)
    // =========================
    private void BTNSCANNEPALETTE_Clicked(object sender, EventArgs e)
    {
    }

    private void BTNSORTIE_Clicked(object sender, EventArgs e)
    {
    }
}