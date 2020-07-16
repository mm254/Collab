using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace IvA
{
    // Programmstart
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Lädt die Standardkonfigurationen aus der Datei Startup.cs
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
