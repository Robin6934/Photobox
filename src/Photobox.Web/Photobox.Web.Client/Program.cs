using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Photobox.Web.Client.MouseService;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});

builder
    .Services.AddSingleton<MouseService>()
    .AddSingleton<IMouseService>(ff => ff.GetRequiredService<MouseService>());

// Log.Logger = new LoggerConfiguration()
//                 .MinimumLevel.Debug()
//                 .WriteTo.BrowserConsole()
//                 .WriteTo.Seq("http://localhost:5341")
//                 .Enrich.FromLogContext()
//                 .Enrich.WithEnvironmentName()
//                 .Enrich.WithMachineName()
//                 .Enrich.WithProperty("Source", "Client")
//                 .CreateLogger();

await builder.Build().RunAsync();
