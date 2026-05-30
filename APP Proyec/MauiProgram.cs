using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using APP_Proyec.Services;

namespace APP_Proyec
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Registrar HttpClient y servicios para consumir la API
            // Por defecto se usa http://10.0.2.2:5000/ (emulador Android). Cambia si es necesario.
            builder.Services.AddHttpClient("Api", client =>
            {
                // API en APIs proyec usa puerto 63011 en launchSettings.json (http)
                client.BaseAddress = new Uri("http://10.0.2.2:63011/");
            });

            builder.Services.AddSingleton<IApiService, ApiService>();
            // Registrar páginas para inyección de dependencias
            builder.Services.AddTransient<Views.ProductsPage>();
            builder.Services.AddTransient<Views.TopSellingPage>();
            builder.Services.AddTransient<Views.OrdersPage>();
            builder.Services.AddTransient<Views.PendingAccountsPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // Exponer el ServiceProvider para resolución desde constructores XAML
            App.Services = app.Services;

            return app;
        }
    }
}
