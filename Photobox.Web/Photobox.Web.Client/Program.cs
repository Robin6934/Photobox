using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.BrowserConsole()
                .WriteTo.Seq("http://localhost:5341")
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Source", "Client")
                .CreateLogger();

await builder.Build().RunAsync();
