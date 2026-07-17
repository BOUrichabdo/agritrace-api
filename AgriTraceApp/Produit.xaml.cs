using AgriTraceApp.Models;
using AgriTraceApp.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace AgriTraceApp;

public partial class Produit : ContentPage
{
    private bool isEditMode = false;
    private int selectedId = 0;
    private ProduitService _service = new ProduitService();
    private List<ParcelleModel>? _parcelle;
    private List<ModeleVariete> _variete = new();
    private List<ProduitModel> _produit = new();
    private CancellationTokenSource _cts;
    private int _societeId;  
    public Produit()
    {
        InitializeComponent();
        _societeId = Preferences.Get("societeId", 0);

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadData();
    }

    private void TXT_PRODUIT_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TXT_PRODUIT.Text))
            return;

        string text = TXT_PRODUIT.Text;
        string formatted = char.ToUpper(text[0]) + text.Substring(1);

        if (TXT_PRODUIT.Text != formatted)
            TXT_PRODUIT.Text = formatted;
    }

    private void ClearForm()
    {
        TXT_PRODUIT.Text = "";
        isEditMode = false;
        selectedId = 0;
        CMBPARCELLE.SelectedItem = null;
        CMBVARIETE.SelectedItem = null;
        TXT_CATEGORIE.Text = "";  
        VALIDER.Text = "Ajouter";
        VALIDER.BackgroundColor = Color.FromArgb("#2E7D32");
        ProduitError.IsVisible = false;
        ParcelleError.IsVisible = false;
        VarieteError.IsVisible = false;
    }

    private async Task LoadData()
    {
        // Charger les produits filtrés par société
        _produit = await _service.GetProduit(_societeId);
        ProduitList.ItemsSource = _produit;

        // Charger les listes déroulantes (parcelles et variétés) filtrées
        await LoadParcelle();
        await LoadVariete();
    }

    private async Task LoadParcelle()
    {
        _parcelle = await _service.GetParcelle(_societeId);
        CMBPARCELLE.ItemsSource = _parcelle;
    }

    private async Task LoadVariete()
    {
        _variete = await _service.GetVarietes(_societeId);
        CMBVARIETE.ItemsSource = _variete;
    }

    private async void VALIDER_Clicked(object sender, EventArgs e)
    {
        bool isValid = true;
        ProduitError.IsVisible = false;
        ParcelleError.IsVisible = false;
        VarieteError.IsVisible = false;

        // Nom
        if (string.IsNullOrWhiteSpace(TXT_PRODUIT.Text))
        {
            ProduitError.Text = "Nom obligatoire";
            ProduitError.IsVisible = true;
            isValid = false;
        }

        var selectParcelle = CMBPARCELLE.SelectedItem as ParcelleModel;
        if (selectParcelle == null)
        {
            ParcelleError.Text = "Veuillez choisir une parcelle";
            ParcelleError.IsVisible = true;
            isValid = false;
        }

        var selectVariete = CMBVARIETE.SelectedItem as ModeleVariete;
        if (selectVariete == null)
        {
            VarieteError.Text = "Veuillez choisir une variété";
            VarieteError.IsVisible = true;
            isValid = false;
        }

        if (!isValid) return;

        if (isEditMode)
        {
            var produit = new ProduitModel
            {
                Id = selectedId,
                Nom = TXT_PRODUIT.Text,
                ParcelleId = selectParcelle.Id,
                VarieteId = selectVariete.Id
            };
            await _service.UpdateProduit(produit, _societeId);
            await Snackbar.Make("Modifié avec succès ✏️").Show();
        }
        else
        {
            try
            {
                var produit = new ProduitModel
                {
                    Nom = TXT_PRODUIT.Text,
                    ParcelleId = selectParcelle.Id,
                    VarieteId = selectVariete.Id
                };
                await _service.AddProduit(produit, _societeId);
                await Snackbar.Make("Produit ajouté avec succès ✅",
                    duration: TimeSpan.FromSeconds(3),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.Green,
                        TextColor = Colors.White
                    }).Show();
            }
            catch (Exception ex)
            {
                await Snackbar.Make("Erreur lors de l'ajout ❌",
                    duration: TimeSpan.FromSeconds(3),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.Red,
                        TextColor = Colors.White
                    }).Show();
            }
        }

        ClearForm();
        await LoadData();
    }

    private async void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        try
        {
            await Task.Delay(300, _cts.Token);
            string keyword = e.NewTextValue?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(keyword))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ProduitList.ItemsSource = _produit;
                });
                return;
            }

            var result = _produit
                .Where(f => !string.IsNullOrWhiteSpace(f.Nom) &&
                            f.Nom.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                ProduitList.ItemsSource = result;
            });
        }
        catch (TaskCanceledException) { }
    }

    private void SwipeItem_Invoked(object sender, EventArgs e)
    {
        var swipe = sender as SwipeItem;
        var produit = (ProduitModel)swipe.CommandParameter;

        TXT_PRODUIT.Text = produit.Nom;
        TXT_CATEGORIE.Text = produit.CategorieNom;

        isEditMode = true;
        selectedId = produit.Id;

        // Sélectionner les items dans les combos
        if (_parcelle != null)
            CMBPARCELLE.SelectedItem = _parcelle.FirstOrDefault(p => p.Id == produit.ParcelleId);
        if (_variete != null)
            CMBVARIETE.SelectedItem = _variete.FirstOrDefault(v => v.Id == produit.VarieteId);

        VALIDER.Text = "Modifier";
        VALIDER.BackgroundColor = Colors.Orange;
    }

    private async void SwipeItem_Invoked_1(object sender, EventArgs e)
    {
        var swipe = sender as SwipeItem;
        var produit = (ProduitModel)swipe.CommandParameter;

        bool confirm = await DisplayAlert("Supprimer ?", $"Supprimer {produit.Nom} ?", "Oui", "Non");
        if (!confirm) return;

        await _service.DeleteProduit(produit.Id, _societeId);
        await LoadData();
        await Snackbar.Make("Produit supprimé 🗑").Show();
    }

    private void CMBVARIETE_SelectedIndexChanged(object sender, EventArgs e)
    {
        var variete = CMBVARIETE.SelectedItem as ModeleVariete;
        if (variete != null)
        {
            TXT_CATEGORIE.Text = variete.CategorieNom;
        }
        else
        {
            TXT_CATEGORIE.Text = "";
        }
    }

    private void TXT_CATEGORIE_TextChanged(object sender, TextChangedEventArgs e)
    {

    }
}













































//using AgriTraceApp.Models;
//using AgriTraceApp.Services;
//using CommunityToolkit.Maui.Alerts;
//using CommunityToolkit.Maui.Core;

//namespace AgriTraceApp;

//public partial class Produit : ContentPage
//{

//    // varible pour gerer modification 
//    private bool isEditMode = false;
//    private int selectedId = 0;
//    // instance service 
//    private ProduitService _service = new ProduitService();
//    // objet Model agriculteur 
//    private List<ParcelleModel>? _parcelle;

//    private List<ModeleVariete> _variete = new();

//    private List<ProduitModel> _produit = new();

//    private CancellationTokenSource _cts;

//    private int _societeId;
//    public Produit()
//    {
//        InitializeComponent();

//        _societeId = Preferences.Get("societeId", 0);

//    }

//    protected override async void OnAppearing()
//    {
//        base.OnAppearing();
//        await LoadData();


//    }


//    private void TXT_PRODUIT_TextChanged(object sender, TextChangedEventArgs e)
//    {

//        if (string.IsNullOrWhiteSpace(TXT_PRODUIT.Text))
//            return;

//        string text = TXT_PRODUIT.Text;

//        // première lettre majuscule de chaque partie de fabricatioon 
//        string formatted =
//            char.ToUpper(text[0]) + text.Substring(1);

//        // éviter boucle infinie
//        if (TXT_PRODUIT.Text != formatted)
//        {
//            TXT_PRODUIT.Text = formatted;
//        }

//    }

//    private void ClearForm()
//    {
//        TXT_PRODUIT.Text = "";

//        isEditMode = false;
//        selectedId = 0;

//        CMBPARCELLE.SelectedItem = null; // 🔥 reset picker
//        CMBVARIETE.SelectedItem = null; // 🔥 reset picker



//        VALIDER.Text = "Ajouter";
//        VALIDER.BackgroundColor = Color.FromArgb("#2E7D32");

//        ProduitError.IsVisible = false;
//        ParcelleError.IsVisible = false;

//        VarieteError.IsVisible = false;

//    }

//    private async Task LoadData()
//    {
//        //// remplire liste fereme
//        //var data = await _service.GetFermes();
//        //ListeFerme.ItemsSource = data;
//        //// remplire picker Agriculteur
//        //await LoadAgriculteurs();


//        // 🔥 stocker données dans la liste principale
//        _produit = await _service.Getproduit();

//        // 🔥 afficher
//        ProduitList.ItemsSource = _produit;




//        // 🔥 charger agriculteurs
//        await Loadparcelle();

//        await Loadvariete();


//    }

//    private async Task Loadparcelle()
//    {
//        _parcelle = await _service.GetParcelle();

//        CMBPARCELLE.ItemsSource = _parcelle;
//    }



//    private async Task Loadvariete()
//    {
//        _variete = await _service.GetVarietes();

//        CMBVARIETE.ItemsSource = _variete;
//    }




//    private async void VALIDER_Clicked(object sender, EventArgs e)
//    {

//        // controle de saisie 
//        bool isValid = true;
//        // RESET ERRORS
//        ProduitError.IsVisible = false;
//        ParcelleError.IsVisible = false;
//        VarieteError.IsVisible = false;

//        // combo agriculteur


//        // 🔴 NOM
//        if (string.IsNullOrWhiteSpace(TXT_PRODUIT.Text))
//        {
//            ProduitError.Text = "Nom obligatoire";
//            ProduitError.IsVisible = true;

//            isValid = false;
//        }
//        var selectparcelle = CMBPARCELLE.SelectedItem as ParcelleModel;

//        if (selectparcelle == null)
//        {
//            ParcelleError.Text = "Veuillez choisir parcelle";
//            ParcelleError.IsVisible = true;
//            isValid = false;
//        }


//        var selectvariete = CMBVARIETE.SelectedItem as ModeleVariete;

//        if (selectvariete == null)
//        {
//            VarieteError.Text = "Veuillez choisir variété";
//            VarieteError.IsVisible = true;
//            isValid = false;
//        }
//        // ❌ STOP SI ERREUR
//        if (!isValid)
//            return;

//        // verification action 
//        if (isEditMode)
//        {
//            // 🔄 UPDATE
//            var agri = new ProduitModel
//            {
//                Id = selectedId,
//                Nom = TXT_PRODUIT.Text,
//                ParcelleId = selectparcelle.Id,
//                VarieteId = selectvariete.Id
//            };
//            await _service.UpdateProduit(agri);
//            await Snackbar.Make("Modifié avec succès ✏️").Show();
//        }
//        // insertion mode 
//        else
//        {
//            try
//            {
//                var Ferme = new ProduitModel
//                {
//                    Nom = TXT_PRODUIT.Text,
//                    ParcelleId = selectparcelle.Id,
//                    VarieteId = selectvariete.Id

//                };

//                await _service.Addproduit(Ferme);
//                var snackbar = Snackbar.Make("Produit ajouté avec succès ✅", duration: TimeSpan.FromSeconds(3),
//                    visualOptions: new SnackbarOptions
//                    {
//                        BackgroundColor = Colors.Green,
//                        TextColor = Colors.White
//                    });

//                await snackbar.Show();
//            }
//            catch (Exception ex)
//            {
//                var snackbar = Snackbar.Make("Erreur lors de l'ajout ❌", duration: TimeSpan.FromSeconds(3),
//                    visualOptions: new SnackbarOptions
//                    {
//                        BackgroundColor = Colors.Red,
//                        TextColor = Colors.White
//                    });

//                await snackbar.Show();


//            }

//        }
//        // Actualiser
//        ClearForm();
//        await LoadData();

//    }

//    private async void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
//    {

//        // 🔥 annuler ancienne recherche
//        _cts?.Cancel();
//        _cts = new CancellationTokenSource();

//        try
//        {
//            // debounce
//            await Task.Delay(300, _cts.Token);

//            string keyword = e.NewTextValue?.Trim() ?? "";

//            // 🔄 afficher tout
//            if (string.IsNullOrWhiteSpace(keyword))
//            {
//                MainThread.BeginInvokeOnMainThread(() =>
//                {
//                    ProduitList.ItemsSource = _produit;
//                });

//                return;
//            }

//            // 🔎 filtrage
//            var result = _produit
//                .Where(f =>
//                    !string.IsNullOrWhiteSpace(f.Nom) &&
//                    f.Nom.Contains(keyword,
//                        StringComparison.OrdinalIgnoreCase))
//                .ToList();

//            // 🔥 update UI
//            MainThread.BeginInvokeOnMainThread(() =>
//            {
//                ProduitList.ItemsSource = result;
//            });
//        }
//        catch (TaskCanceledException)
//        {

//        }

//    }

//    private void SwipeItem_Invoked(object sender, EventArgs e)
//    {

//        var swipe = sender as SwipeItem;
//        var agri = (ProduitModel)swipe.CommandParameter;

//        // remplir champs
//        TXT_PRODUIT.Text = agri.Nom;
//        TXT_CATEGORIE.Text = agri.CategorieNom;
//        // mode edit
//        isEditMode = true;
//        selectedId = agri.Id;

//        int selectvariete = agri.VarieteId;

//        int selectparcelle = agri.ParcelleId;



//        CMBPARCELLE.SelectedItem = _parcelle
//    .FirstOrDefault(a => a.Id == selectparcelle);


//        CMBVARIETE.SelectedItem = _variete
//.FirstOrDefault(a => a.Id == selectvariete);

//        // changer bouton UI
//        VALIDER.Text = "Modifier";
//        VALIDER.BackgroundColor = Colors.Orange;

//    }

//    private async void SwipeItem_Invoked_1(object sender, EventArgs e)
//    {

//        var swipe = sender as SwipeItem;
//        var agri = (ProduitModel)swipe.CommandParameter;

//        bool confirm = await DisplayAlert(
//            "Supprimer ?",
//            $"Supprimer {agri.Nom} ?",
//            "Oui",
//            "Non");

//        if (!confirm)
//            return;

//        await _service.Deleteproduit(agri.Id);
//        await LoadData();

//        //// afficher infos supprimées dans formulaire (comme tu veux)
//        //TXT_AGRICULTEUR.Text = agri.Nom;
//        //TELE.Text = agri.Telephone;
//        //ADRESSE.Text = agri.Adresse;

//        await Snackbar.Make("Ferme supprimé 🗑").Show();

//    }

//    private void TXT_CATEGORIE_TextChanged(object sender, TextChangedEventArgs e)
//    {



//    }

//    private void CMBVARIETE_SelectedIndexChanged(object sender, EventArgs e)
//    {

//        var variete = CMBVARIETE.SelectedItem as ModeleVariete;

//        if (variete != null)
//        {
//            TXT_CATEGORIE.Text = variete.CategorieNom;
//        }

//    }
//}