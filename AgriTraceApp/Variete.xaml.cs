using AgriTraceApp.Models;
using AgriTraceApp.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace AgriTraceApp;

public partial class Variete : ContentPage
{

    private bool isEditMode = false;
    private int selectedId = 0;

    // instance service 
    //private ApiService _service = new ApiService();


    private VarieteService _servicevariete = new VarieteService();


    private CategorieService _servicecategorie = new CategorieService();

    // objet Model agriculteur 
    private List<ModeleCategorie>? _categorie;

    private List<ModeleVariete> _variete = new();
    private CancellationTokenSource _cts;


    // instance service 
    public Variete()
	{
		InitializeComponent();
	}

    private async Task LoadData()
    {
        //// remplire liste fereme
        //var data = await _service.GetFermes();
        //ListeFerme.ItemsSource = data;
        //// remplire picker Agriculteur
        //await LoadAgriculteurs();


        // 🔥 stocker données dans la liste principale
        _variete = await _servicevariete.GetVarietes();

        // 🔥 afficher
        VarieteList.ItemsSource = _variete;

        // 🔥 charger agriculteurs
        await LoadCategories();

    }

    private void ClearForm()
    {
        TXT_VARIETE.Text = "";

        isEditMode = false;
        selectedId = 0;

        CMBOCATEGORIE.SelectedItem = null; // 🔥 reset picker


        VALIDER.Text = "Ajouter";
        VALIDER.BackgroundColor = Color.FromArgb("#2E7D32");

        NomError.IsVisible = false;

        CategorieError.IsVisible = false;

    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadData();


    }

    // lod donne  

    // remplire liste categorie 

    private async Task LoadCategories()
    {
        _categorie = await _servicecategorie.GetCategorie();

        CMBOCATEGORIE.ItemsSource = _categorie;
    }

    private async void VALIDER_Clicked(object sender, EventArgs e)
    {

        // controle de saisie 
        bool isValid = true;
        // RESET ERRORS
        CategorieError.IsVisible = false;

        NomError.IsVisible = false;

        // combo agriculteur


        // 🔴 NOM
        if (string.IsNullOrWhiteSpace(TXT_VARIETE.Text))
        {
            NomError.Text = "Nom obligatoire";
            NomError.IsVisible = true;
            NomError.Focus();

            isValid = false;
        }
        var selectedAgri = CMBOCATEGORIE.SelectedItem as ModeleCategorie;

        if (selectedAgri == null)
        {
            CategorieError.Text = "Veuillez choisir catégories";
            CategorieError.IsVisible = true;
            isValid = false;
        }
        // ❌ STOP SI ERREUR
        if (!isValid)
            return;

        // verification action 
        if (isEditMode)
        {
            // 🔄 UPDATE
            var agri = new ModeleVariete
            {
                Id = selectedId,
                Intitule = TXT_VARIETE.Text,
                CategorieId = selectedAgri.Id
            };
            await _servicevariete.UpdateVariete(agri);
            await Snackbar.Make("Modifié avec succès ✏️").Show();
        }
        // insertion mode 
        else
        {
            try
            {
                var Ferme = new ModeleVariete
                {
                    Intitule = TXT_VARIETE.Text.Trim(),
                    CategorieId = selectedAgri.Id

                };

                await _servicevariete.AddVariete(Ferme);
                var snackbar = Snackbar.Make("Variété ajouté avec succès ✅", duration: TimeSpan.FromSeconds(3),
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
                    VarieteList.ItemsSource = _variete;
                });

                return;
            }

            // 🔎 filtrage
            var result = _variete
                .Where(f =>
                    !string.IsNullOrWhiteSpace(f.Intitule) &&
                    f.Intitule.Contains(keyword,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

            // 🔥 update UI
            MainThread.BeginInvokeOnMainThread(() =>
            {
                VarieteList.ItemsSource = result;
            });
        }
        catch (TaskCanceledException)
        {

        }

    }

    private void SwipeItem_Invoked(object sender, EventArgs e)
    {
        var swipe = sender as SwipeItem;
        var agri = (ModeleVariete)swipe.CommandParameter;

        // remplir champs
        TXT_VARIETE.Text = agri.Intitule;



        // mode edit
        isEditMode = true;
        selectedId = agri.Id;

        int selectcategorie = agri.CategorieId;


        CMBOCATEGORIE.SelectedItem = _categorie
    .FirstOrDefault(a => a.Id == selectcategorie);

        // changer bouton UI
        VALIDER.Text = "Modifier";
        VALIDER.BackgroundColor = Colors.Orange;

    }

    private async void SwipeItem_Invoked_1(object sender, EventArgs e)
    {

        var swipe = sender as SwipeItem;
        var agri = (ModeleVariete)swipe.CommandParameter;

        bool confirm = await DisplayAlert(
            "Supprimer ?",
            $"Supprimer {agri.Intitule} ?",
            "Oui",
            "Non");

        if (!confirm)
            return;

        await _servicevariete.DeleteVariete(agri.Id);
        await LoadData();

        //// afficher infos supprimées dans formulaire (comme tu veux)
        //TXT_AGRICULTEUR.Text = agri.Nom;
        //TELE.Text = agri.Telephone;
        //ADRESSE.Text = agri.Adresse;

        await Snackbar.Make("Variété supprimé 🗑").Show();

    }

    private void TXT_VARIETE_TextChanged(object sender, TextChangedEventArgs e)
    {

        if (string.IsNullOrWhiteSpace(TXT_VARIETE.Text))
            return;

        string text = TXT_VARIETE.Text;

        // première lettre majuscule
        string formatted =
            char.ToUpper(text[0]) + text.Substring(1);

        // éviter boucle infinie
        if (TXT_VARIETE.Text != formatted)
        {
            TXT_VARIETE.Text = formatted;
        }

    }
}