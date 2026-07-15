namespace AgriTraceApp;

public partial class WelcomePage : ContentPage
{
    public WelcomePage()
    {
        InitializeComponent();
    }

    private async void BTNLOGIN_Clicked(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new Login());


    }

    private async void BTNSOCITE_Clicked(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new Societe());



    }
}