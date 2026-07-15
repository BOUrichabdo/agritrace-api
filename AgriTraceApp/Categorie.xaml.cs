using AgriTraceApp.Models;
using AgriTraceApp.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace AgriTraceApp;

public partial class Categorie : ContentPage
{

    // varible pour gerer modification 
    private bool isEditMode = false;
    private int selectedId = 0;

    // filtrage 
    private List<ModeleCategorie> _categorie = new();
    private CancellationTokenSource _cts;

    //private ApiService _service = new ApiService();

    private CategorieService _categorieservice = new CategorieService();

    public Categorie()
    {
        InitializeComponent();
    }

    private async Task LoadData()
    {
        //var data = await _service.GetAgriculteurs();
        //AgriculteurList1.ItemsSource = data;


        // 🔥 stocker données dans la liste principale
        _categorie = await _categorieservice.GetCategorie();

        // 🔥 afficher
        CategorieList.ItemsSource = _categorie;


    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadData();
    }



    private void TXT_CATEGORIE_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TXT_CATEGORIE.Text))
            return;

        string text = TXT_CATEGORIE.Text;

        // première lettre majuscule
        string formatted =
            char.ToUpper(text[0]) + text.Substring(1);

        // éviter boucle infinie
        if (TXT_CATEGORIE.Text != formatted)
        {
            TXT_CATEGORIE.Text = formatted;
        }


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
                    CategorieList.ItemsSource = _categorie;
                });

                return;
            }

            // 🔎 filtrage
            var result = _categorie
                .Where(f =>
                    !string.IsNullOrWhiteSpace(f.Intitule) &&
                    f.Intitule.Contains(keyword,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

            // 🔥 update UI
            MainThread.BeginInvokeOnMainThread(() =>
            {
                CategorieList.ItemsSource = result;
            });
        }
        catch (TaskCanceledException)
        {

        }

    }

    private void SwipeItem_Invoked(object sender, EventArgs e)
    {

        var swipe = sender as SwipeItem;
        var agri = (ModeleCategorie)swipe.CommandParameter;

        // remplir champs
        TXT_CATEGORIE.Text = agri.Intitule;

        // mode edit
        isEditMode = true;
        selectedId = agri.Id;

        // changer bouton UI
        VALIDER.Text = "Modifier";
        VALIDER.BackgroundColor = Colors.Orange;

    }

    private async void SwipeItem_Invoked_1(object sender, EventArgs e)
    {

        var swipe = sender as SwipeItem;
        var agri = (ModeleCategorie)swipe.CommandParameter;

        bool confirm = await DisplayAlert(
            "Supprimer ?",
            $"Supprimer {agri.Intitule} ?",
            "Oui",
            "Non");

        if (!confirm)
            return;

        await _categorieservice.DeletCategorie(agri.Id);



        await LoadData();

        //// afficher infos supprimées dans formulaire (comme tu veux)
        //TXT_AGRICULTEUR.Text = agri.Nom;
        //TELE.Text = agri.Telephone;
        //ADRESSE.Text = agri.Adresse;

        await Snackbar.Make("Catégorie supprimé 🗑").Show();

    }

    private void ClearForm()
    {
        TXT_CATEGORIE.Text = "";

        isEditMode = false;
        selectedId = 0;

        VALIDER.Text = "Ajouter";
        VALIDER.BackgroundColor = Color.FromArgb("#2E7D32");
    }




    private async void VALIDER_Clicked(object sender, EventArgs e)
    {

        // controle de saisie 
        bool isValid = true;
        // RESET ERRORS
        NomError.IsVisible = false;

        // 🔴 NOM
        if (string.IsNullOrWhiteSpace(TXT_CATEGORIE.Text))
        {
            NomError.Text = "Nom obligatoire";
            NomError.IsVisible = true;
            isValid = false;
        }
        // ❌ STOP SI ERREUR
        if (!isValid)
            return;

        // verification action 
        if (isEditMode)
        {
            // 🔄 UPDATE
            var agri = new ModeleCategorie
            {
                Id = selectedId,
                Intitule = TXT_CATEGORIE.Text


            };
            await _categorieservice.UpdateCategorie(agri);
            await Snackbar.Make("Modifié avec succès ✏️").Show();
        }
        else
        {
            try
            {
                var categorie = new ModeleCategorie
                {
                    Intitule = TXT_CATEGORIE.Text.Trim()


                };

                await _categorieservice.AddCategorie(categorie);

                var snackbar = Snackbar.Make(
                    "Catégorie ajouté avec succès ✅",
                    duration: TimeSpan.FromSeconds(3),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.Green,
                        TextColor = Colors.White
                    });

                await snackbar.Show();
                //// reset form
                //TXT_AGRICULTEUR.Text = "";
                //TELE.Text = "";
                //ADRESSE.Text = "";

                //await LoadData();


            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make(
                    "Erreur lors de l'ajout ❌",
                    duration: TimeSpan.FromSeconds(3),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.Red,
                        TextColor = Colors.White
                    });

                await snackbar.Show();


            }

        }

        // reset
        ClearForm();
        await LoadData();

    }
}