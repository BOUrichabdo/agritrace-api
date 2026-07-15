using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using UraniumUI;
using ZXing.Net.Maui.Controls;

namespace AgriTraceApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();


            builder
                      .UseMauiApp<App>()
                      .UseMauiCommunityToolkit()
                      .UseUraniumUI()          // <-- Ajouter cette ligne
                       .UseUraniumUIMaterial()   // <-- Pour le thème Material



                      .UseBarcodeReader()
                      .ConfigureFonts(fonts =>
                      {
                          fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                      });

            // ✅ AJOUTER ICI

            //builder.Services.AddSingleton(new HttpClient
            //{
            //    BaseAddress = new Uri("https://mc4h5hwm-5171.uks1.devtunnels.ms/")
            //});  

#if DEBUG
            builder.Logging.AddDebug();

            builder.Services.AddSingleton<PdfService>();
#endif

            return builder.Build();
        }
    }
}