namespace AgriTraceApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private void OnCounterClicked(object? sender, EventArgs e)
        {

        }
        // affichage agriculteur
        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            //await Shell.Current.GoToAsync("///agriculteurs");

            await Navigation.PushAsync(new Agriculteur());

        }
        // afficher fereme
        private async void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
        {
            //await Shell.Current.GoToAsync("///ferme");

            await Navigation.PushAsync(new Ferme());
        }

        // afficher parcelle
        private void TapGestureRecognizer_Tapped_2(object sender, TappedEventArgs e)
        {

            //await Shell.Current.GoToAsync("///parcelle");


        }
        // afficjage produit 
        private async void TapGestureRecognizer_Tapped_3(object sender, TappedEventArgs e)
        {

            //await Shell.Current.GoToAsync("///produit");

            await Navigation.PushAsync(new Produit());

        }
        // affichage categorie
        private async void TapGestureRecognizer_Tapped_4(object sender, TappedEventArgs e)
        {

            //await Shell.Current.GoToAsync("///categorie");

            await Navigation.PushAsync(new Categorie());


        }
        private void TapGestureRecognizer_Tapped_5(object sender, TappedEventArgs e)
        {

        }
        private void TapGestureRecognizer_Tapped_6(object sender, TappedEventArgs e)
        {

        }
        private void TapGestureRecognizer_Tapped_7(object sender, TappedEventArgs e)
        {



        }
        private void TapGestureRecognizer_Tapped_8(object sender, TappedEventArgs e)
        {




        }
        private async void TapGestureRecognizer_Tapped_9(object sender, TappedEventArgs e)
        {

            //await Shell.Current.GoToAsync("///variete");

            await Navigation.PushAsync(new Variete());





        }
        // AFFICHAGE FEREME 
        private async void TapGestureRecognizer_Tapped_10(object sender, TappedEventArgs e)
        {

            //await Shell.Current.GoToAsync("///ferme");

            await Navigation.PushAsync(new Ferme());


        }
        private void TapGestureRecognizer_Tapped_11(object sender, TappedEventArgs e)
        {



        }
        private async void TapGestureRecognizer_Tapped_12(object sender, TappedEventArgs e)
        {

            //await Shell.Current.GoToAsync("///agriculteurs");

            await Navigation.PushAsync(new Agriculteur());


        }
        private async void BTNPARCELLE_Tapped(object sender, TappedEventArgs e)
        {

            //await Shell.Current.GoToAsync("///parcelle");

            await Navigation.PushAsync(new Parcelle());



        }

        private void Sortie_Tapped(object sender, TappedEventArgs e)
        {

        }

        private void Sortie_Tapped_1(object sender, TappedEventArgs e)
        {

        }

        private void TapGestureRecognizer_Tapped_13(object sender, TappedEventArgs e)
        {

        }
        private async void SORTIE_Tapped_2(object sender, TappedEventArgs e)
        {


            //await Shell.Current.GoToAsync("///etiquetteferme"); 

            await Navigation.PushAsync(new EtiquetteFerme());




        }
        private async void RECPTION_Tapped(object sender, TappedEventArgs e)
        {

            //await Shell.Current.GoToAsync("///reception");

            await Navigation.PushAsync(new Reception());



        }
        private async void Sortiestock_Tapped(object sender, TappedEventArgs e)
        {

            //await Shell.Current.GoToAsync("///_sortie");


            await Navigation.PushAsync(new SORTIE());



        }
    }
}
