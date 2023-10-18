using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorApp;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        var services = builder.Services;
        RegisterHttpClient(builder, services);

        builder.Services.AddOidcAuthentication(options =>
        {
            options.ProviderOptions.MetadataUrl = "http://localhost:8080/realms/myrealm/.well-known/openid-configuration";
            options.ProviderOptions.Authority = "http://localhost:8080/realms/myrealm";
            options.ProviderOptions.ClientId = "blazor-client";
            options.ProviderOptions.ResponseType = "code";

            options.UserOptions.NameClaim = "preferred_username";
            options.UserOptions.RoleClaim = "roles";
            options.UserOptions.ScopeClaim = "scope";
        });

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        var app = builder.Build();

        await app.RunAsync();
    }

    private static void RegisterHttpClient(
        WebAssemblyHostBuilder builder,
        IServiceCollection services)
    {
        var httpClientName = "Default";

        services.AddHttpClient(httpClientName,
            client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

        services.AddScoped(
            sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient(httpClientName));
    }
}
