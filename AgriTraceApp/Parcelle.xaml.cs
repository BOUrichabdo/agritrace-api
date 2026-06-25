
using AgriTraceApp.Models;
using AgriTraceApp.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;


namespace AgriTraceApp;

public partial class Parcelle : ContentPage
{

    private bool isEditMode = false;
    private int selectedId = 0;

    // instance service 
    //private ApiService _service = new ApiService();

    
    
    private ParcelleService _servicepareclle = new ParcelleService();

    private FermeService _serviceferme = new FermeService();

    



    // objet Model agriculteur 
    private List<FermeModele>? _ferme;

    private List<ParcelleModel> _parcelle = new();
    private CancellationTokenSource _cts;

    private int societeId; 

    public Parcelle()
	{
		InitializeComponent();
        societeId = Preferences.Get("societeId", 0);

    }

    private async Task LoadData()
    {
        //// remplire liste fereme
        //var data = await _service.GetFermes();
        //ListeFerme.ItemsSource = data;
        //// remplire picker Agriculteur
        //await LoadAgriculteurs();


        // 🔥 stocker données dans la liste principale
        _parcelle = await _servicepareclle.GetParcelle(societeId);

        // 🔥 afficher
        LISTEPARCELLE.ItemsSource = _parcelle;

        // 🔥 charger agriculteurs
        await LoadFerme();

    }

    private void ClearForm()
    {
        ENTRYPARCELLE.Text = "";

        isEditMode = false;
        selectedId = 0;

        PICKERFERME.SelectedItem = null; // 🔥 reset picker


        VALIDER.Text = "Ajouter";
        VALIDER.BackgroundColor = Color.FromArgb("#2E7D32");

        ERREURFERME.IsVisible = false;

        ERREURPARCELLE.IsVisible = false;

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadData();


    }


    private async Task LoadFerme()
    {
        _ferme = await _serviceferme.GetFermes(societeId);

        PICKERFERME.ItemsSource = _ferme;
    }

    private void ENTRYNAMEPARCELLE_TextChanged(object sender, TextChangedEventArgs e)
    {

        if (string.IsNullOrWhiteSpace(ENTRYPARCELLE.Text))
            return;

        string text = ENTRYPARCELLE.Text;

        // première lettre majuscule
        string formatted =
            char.ToUpper(text[0]) + text.Substring(1);

        // éviter boucle infinie
        if (ENTRYPARCELLE.Text != formatted)
        {
            ENTRYPARCELLE.Text = formatted;
        }



    }

    private void ENTRYPARCELLE_TextChanged(object sender, TextChangedEventArgs e)
    {

        if (string.IsNullOrWhiteSpace(ENTRYPARCELLE.Text))
            return;

        string text = ENTRYPARCELLE.Text;

        // première lettre majuscule
        string formatted =
            char.ToUpper(text[0]) + text.Substring(1);

        // éviter boucle infinie
        if (ENTRYPARCELLE.Text != formatted)
        {
            ENTRYPARCELLE.Text = formatted;
        }

    }

    private async void VALIDER_Clicked(object sender, EventArgs e)
    {

        // controle de saisie 
        bool isValid = true;
        // RESET ERRORS
        ERREURFERME.IsVisible = false;

        ERREURPARCELLE.IsVisible = false;

        // combo agriculteur


        // 🔴 NOM
        if (string.IsNullOrWhiteSpace(ENTRYPARCELLE.Text))
        {
            ERREURPARCELLE.Text = "Nom obligatoire";
            ERREURPARCELLE.IsVisible = true;
            ENTRYPARCELLE.Focus();

            isValid = false;
        }
        var selectedAgri = PICKERFERME.SelectedItem as FermeModele;

        if (selectedAgri == null)
        {
            ERREURFERME.Text = "Veuillez choisir ferme";
            ERREURFERME.IsVisible = true;
            isValid = false;
        }
        // ❌ STOP SI ERREUR
        if (!isValid)
            return;

        // verification action 
        if (isEditMode)
        {
            // 🔄 UPDATE
            var agri = new ParcelleModel
            {
                Id = selectedId,
                NomParcelle = ENTRYPARCELLE.Text,
                FermeId = selectedAgri.Id
            };
            await _servicepareclle.UpdateParcelled(agri);
            await Snackbar.Make("Modifié avec succès ✏️").Show();
        }
        // insertion mode 
        else
        {
            try
            {
                var Ferme = new ParcelleModel
                {
                    NomParcelle = ENTRYPARCELLE.Text.Trim(),
                    FermeId = selectedAgri.Id

                };

                await _servicepareclle.Addparcelle(Ferme);
                var snackbar = Snackbar.Make("Parcelle ajouté avec succès ✅", duration: TimeSpan.FromSeconds(3),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.Green,
                        TextColor = Colors.White
                    });

                await snackbar.Show();
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make("Erreur lors de l'ajout ❌", duration: TimeSpan.FromSeconds(3),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.Red,
                        TextColor = Colors.White
                    });

                await snackbar.Show();


            }

        }
        // Actualiser
        ClearForm();
        await LoadData();

    }

    private async void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
    {

        // 🔥 annuler ancienne recherche
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        try
        {
            // debounce
            await Task.Delay(300, _cts.Token);

            string keyword = e.NewTextValue?.Trim() ?? "";

            // 🔄 afficher tout
            if (string.IsNullOrWhiteSpace(keyword))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    LISTEPARCELLE.ItemsSource = _parcelle;
                });

                return;
            }

            // 🔎 filtrage
            var result = _parcelle
                .Where(f =>
                    !string.IsNullOrWhiteSpace(f.NomParcelle) &&
                    f.NomParcelle.Contains(keyword,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

            // 🔥 update UI
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LISTEPARCELLE.ItemsSource = result;
            });
        }
        catch (TaskCanceledException)
        {

        }

    }

    private void UPDATE_Invoked(object sender, EventArgs e)
    {
        var swipe = sender as SwipeItem;
        var agri = (ParcelleModel)swipe.CommandParameter;

        // remplir champs
        ENTRYPARCELLE.Text = agri.NomParcelle;



        // mode edit
        isEditMode = true;
        selectedId = agri.Id;

        int selectcategorie = agri.FermeId;


        PICKERFERME.SelectedItem = _ferme
    .FirstOrDefault(a => a.Id == selectcategorie);

        // changer bouton UI
        VALIDER.Text = "Modifier";
        VALIDER.BackgroundColor = Colors.Orange;

    }

    private async void DELETE_Invoked(object sender, EventArgs e)
    {

        var swipe = sender as SwipeItem;
        var agri = (ParcelleModel)swipe.CommandParameter;

        bool confirm = await DisplayAlert(
            "Supprimer ?",
            $"Supprimer {agri.NomParcelle} ?",
            "Oui",
            "Non");

        if (!confirm)
            return;

        await _servicepareclle.DeletParacelle(agri.Id);
        await LoadData();

        //// afficher infos supprimées dans formulaire (comme tu veux)
        //TXT_AGRICULTEUR.Text = agri.Nom;
        //TELE.Text = agri.Telephone;
        //ADRESSE.Text = agri.Adresse;

        await Snackbar.Make("Variété supprimé 🗑").Show();

    }
}