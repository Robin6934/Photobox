using PhotoboxWeb.Components;
using Microsoft.Extensions.FileProviders;

namespace PhotoboxWeb
{
    public class Program
    {
        public static string PhotoBoxDirectory { get; } = Path.Combine(["C:\\Users", Environment.GetEnvironmentVariable("username") ?? "", "Pictures\\Photobox\\Static"]);
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents();
            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(PhotoBoxDirectory),
                RequestPath = "/Images"
            });

            app.MapControllers();

            app.MapRazorComponents<App>();

            app.Run();
        }
    }
}
