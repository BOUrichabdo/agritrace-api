using AgriTraceApp.DTOs;
using AgriTraceApp.Services;

namespace AgriTraceApp;

public partial class SORTIE : ContentPage
{

    private readonly SortieStockService _service = new();

    private PaletteSortieDto? _palette;

    public SORTIE()
	{
		InitializeComponent();
	}

    private async void BTNSCANNEPALETTE_Clicked(object sender, EventArgs e)
    {

        // controle de saisie

        try
        {
            if (string.IsNullOrWhiteSpace(
                TXT_CODE_PALETTE.Text))
            {
                await DisplayAlert(
                    "Erreur",
                    "Saisir code palette",
                    "OK");

                return;
            }

            _palette = await _service
                .GetPaletteByCode(
                TXT_CODE_PALETTE.Text.Trim());

            if (_palette == null)
            {
                await DisplayAlert(
                    "Erreur",
                    "Palette introuvable",
                    "OK");

                return;
            }

            // =========================
            // AFFICHAGE
            // =========================

            LBL_PALETTE.Text =
                _palette.CodePalette;

            LBL_PRODUIT.Text =
                _palette.Produit;

            LBL_VARIETE.Text =
                _palette.Variete;

            LBL_QTE.Text =
                $"{_palette.QuantiteDisponible} KG";
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Erreur",
                ex.Message,
                "OK");
        }

    }

    private async void BTNSORTIE_Clicked(object sender, EventArgs e)
    {

        try
        {
            // =========================
            // CONTROLES
            // =========================

            if (_palette == null)
            {
                await DisplayAlert(
                    "Erreur",
                    "Scanner palette",
                    "OK");

                return;
            }

            if (string.IsNullOrWhiteSpace(
                TXT_QTE_SORTIE.Text))
            {
                await DisplayAlert(
                    "Erreur",
                    "Saisir quantité",
                    "OK");

                return;
            }

            decimal qte =
                decimal.Parse(TXT_QTE_SORTIE.Text);

            // =========================
            // DTO
            // =========================

            var dto = new CreateSortieStockDto
            {
                CodePalette =
                    _palette.CodePalette,

                QuantiteSortie = qte,

                Utilisateur = "admin",

                Observation =
                    TXT_OBSERVATION.Text ?? ""
            };

            // =========================
            // API
            // =========================

            var result =
                await _service.CreateSortie(dto);

            if (result)
            {
                await DisplayAlert(
                    "Succès",
                    "Sortie stock réussie",
                    "OK");

                ClearForm();
            }
            else
            {
                await DisplayAlert(
                    "Erreur",
                    "Erreur sortie stock",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Erreur",
                ex.Message,
                "OK");
        }

    }

    // actualiser form

    private void ClearForm()
    {
        TXT_CODE_PALETTE.Text = "";

        TXT_QTE_SORTIE.Text = "";

        TXT_DESTINATION.Text = "";

        TXT_TRANSPORT.Text = "";

        TXT_OBSERVATION.Text = "";

        LBL_PALETTE.Text = "";

        LBL_PRODUIT.Text = "";

        LBL_VARIETE.Text = "";

        LBL_QTE.Text = "";

        _palette = null;
    }
}