using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorSampleApp.Client;

internal sealed class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        await builder.Build().RunAsync();
    }
}
