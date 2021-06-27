using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Database.Data;

namespace Database
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            bool migrateOnStartup = Boolean.Parse(Environment.GetEnvironmentVariable("migrateOnStartup") ?? "false");
            if (migrateOnStartup)
            {
                try
                {
                    using (var scope = host.Services.CreateScope())
                    {
                        var services = scope.ServiceProvider;
                        try
                        {
                            var Database = services.GetRequiredService<QAContext>().Database;
                            while (!Database.CanConnect()) ;
                            Database.Migrate();
                        }
                        catch (Exception ex)
                        {
                            services
                                .GetRequiredService<ILogger<Program>>()
                                .LogError(ex, "An error occurred while migrating the database.");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while migrate on startup:", e);
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseStartup<Startup>();
                });
    }
}
