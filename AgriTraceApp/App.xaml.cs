namespace AgriTraceApp
{
    public partial class App : Application
    {
        public  App()
        {
            InitializeComponent();

            // PAGE PRINCIPALE AVEC NAVIGATION
            //MainPage = new NavigationPage(new MainPage())
            //{
            //    BarBackgroundColor = Colors.White,
            //    BarTextColor = Colors.Black
            //};

            MainPage = new NavigationPage(new Historique());


            //MainPage = new NavigationPage(new EtiquetteFerme());


            //MainPage = new NavigationPage(new Reception());



            //MainPage = new NavigationPage(new ListeEtiquettesPage());




        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    //return new Window(new AppShell());


        //}
    }
}