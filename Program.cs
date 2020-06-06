using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace tictactoe_service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    Console.WriteLine($"Using 'appsettings.json' configuration file.");
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                    var environmentName = hostingContext.HostingEnvironment.EnvironmentName;
                    Console.WriteLine($"EnvironmentName: {environmentName}");

                    Console.WriteLine($"Using 'appsettings.{environmentName}.json' configuration file.");
                    config.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

                    config.AddEnvironmentVariables("");

                })
                .ConfigureLogging((context, loggingBuilder) =>
                {
                    loggingBuilder.SetMinimumLevel(LogLevel.Debug);
                    loggingBuilder.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
