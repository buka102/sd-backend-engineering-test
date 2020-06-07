using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tictactoe_service
{
    public class LambdaEntry : Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    {
        /// <summary>
        /// The builder has configuration, logging and Amazon API Gateway already configured. The startup class
        /// needs to be configured in this method using the UseStartup<>() method.
        /// </summary>
        /// <param name="builder"></param>
        protected override void Init(IWebHostBuilder builder)
        {
            builder
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
                .UseStartup<Startup>();
        }
    }
}
