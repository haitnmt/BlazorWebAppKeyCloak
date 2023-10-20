using BlazorApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BlazorApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddOptions();
        builder.Services.AddAuthorizationCore();
        builder.Services.TryAddSingleton<AuthenticationStateProvider, HostAuthenticationStateProvider>();
        builder.Services.TryAddSingleton(sp => (HostAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
        builder.Services.AddTransient<AuthorizedHandler>();
        builder.Services.AddHttpClient("default", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
        builder.Services.AddHttpClient("authorizedClient")
            .AddHttpMessageHandler<AuthorizedHandler>();
        builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("default"));
        builder.Services.AddTransient<FetchWeatherForecastService>();

        await builder.Build().RunAsync();
    }
}
