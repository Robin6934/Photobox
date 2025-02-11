using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Photobox.Web.Client.MouseService;
using Serilog;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

builder.Services.AddSingleton<MouseService>();

builder.Services.AddSingleton<IMouseService>(s => s.GetRequiredService<MouseService>());

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
