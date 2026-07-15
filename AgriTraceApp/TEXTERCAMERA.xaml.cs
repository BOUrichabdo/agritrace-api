


using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace AgriTraceApp;

public partial class TEXTERCAMERA : ContentPage
{
    private bool _isScanning = false;

    public TEXTERCAMERA()
    {
        InitializeComponent();
    }

    private async void OnScanClicked(object sender, EventArgs e)
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

            // Afficher le conteneur de la caméra
            CameraContainer.IsVisible = true;

            // Attendre un peu que l'UI se mette à jour
            await Task.Delay(100);

            // Recréer la caméra pour éviter l'écran noir
            RecreateCameraView();

            // Activer la détection
            _isScanning = true;
            cameraView.IsDetecting = true;

            // Désactiver le bouton pendant le scan
            btnScan.Text = "🔍 SCAN EN COURS...";
            btnScan.IsEnabled = false;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
    }

    private void RecreateCameraView()
    {
        try
        {
            // Créer une nouvelle instance de la caméra
            var newCameraView = new CameraBarcodeReaderView()
            {
                CameraLocation = CameraLocation.Rear,
                IsDetecting = true,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            newCameraView.BarcodesDetected += OnBarcodesDetected;

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

    private async void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
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
            try
            {
                // Désactiver la détection
                cameraView.IsDetecting = false;

                // AFFICHER LE QR CODE DANS L'ENTRY
                string qrCode = result.Value;
                txtQRCode.Text = qrCode;

                // Optionnel : Afficher une alerte en plus
                await DisplayAlert("QR Code Détecté", $"Code: {qrCode}", "OK");

                // Cacher le conteneur de la caméra
                CameraContainer.IsVisible = false;

                // Vider le conteneur pour libérer les ressources
                CameraContainer.Children.Clear();

                // Réinitialiser le bouton
                btnScan.Text = "📷 SCANNER QR CODE";
                btnScan.IsEnabled = true;

                // Optionnel : Vous pouvez maintenant utiliser ce QR code
                // await TraiterQRCode(qrCode);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", ex.Message, "OK");
            }
        });
    }


}




