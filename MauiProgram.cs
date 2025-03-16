using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace TPApp;

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

        builder.Services.AddHttpClient("AuthClient", client =>
        {
            client.BaseAddress = new Uri("https://xxx.xxx.xxx.xxx:7226/");
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        });

        builder.Services.AddTransient<LoginPage>(sp =>
            new LoginPage(sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthClient"))
        );
        builder.Services.AddTransient<RegistrationPage>(sp =>
            new RegistrationPage(sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthClient"))
        );
        builder.Services.AddTransient<MainPage>(sp =>
            new MainPage(sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthClient"))
        );
        builder.Services.AddTransient<AddFoodItemPage>(sp =>
            new AddFoodItemPage(sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthClient"))
        );
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}