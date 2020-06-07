using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Runtime.Internal.Auth;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using tictactoe_service.Configuration;
using tictactoe_service.CQRS.Shared;
using tictactoe_service.Data;
using tictactoe_service.Logic;

namespace tictactoe_service
{
    public class Startup
    {

        // CORS
        readonly static string MyAllowSpecificOrigins = "_myAllowTicTacToeOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.

        public void ConfigureServices(IServiceCollection services)
        {
            GlobalConfigureServices(services, Configuration);
            services.AddSingleton<AWSDynamoConfig>(ctx => {
                var config = ctx.GetRequiredService<IConfiguration>();
                var strAccessKey = config["AWS:AccessKey"];
                var strSecretKey = config["AWS:SecretKey"];
                var strRegion = config["AWS:Region"];
                var strTable = config["AWS:Table"];
                return new AWSDynamoConfig { Region = strRegion, AccessKey = strAccessKey, SecretKey = strSecretKey, Table = strTable };
            });
        }

        public static void GlobalConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers().AddNewtonsoftJson();  //Using newtonJson for better compatability
            services.AddOptions();
            services.AddHttpClient();
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    // TODO:  Move to configuration
                    builder.WithOrigins("*");
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });
            });
            services.AddHttpContextAccessor();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto |
                    ForwardedHeaders.XForwardedHost;
            });

            //mediator
            services.AddMediatR(new System.Reflection.Assembly[1] { typeof(Startup).Assembly });
            services.AddValidatorsFromAssembly(typeof(Startup).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
            services.AddSingleton<IGameEvaluator, BasicGameEvaluator>();
            //services.AddSingleton<IRepository, InMemoryRepository>();
            services.AddSingleton<IRepository, DynamoDbRepository>();

            services.AddSingleton<AmazonDynamoDBClient>(ctx => {
                var config = ctx.GetRequiredService<AWSDynamoConfig>();
                AmazonDynamoDBClient awsDbClient = new AmazonDynamoDBClient(config.AccessKey, config.SecretKey, Amazon.RegionEndpoint.GetBySystemName(config.Region));
                return awsDbClient;
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
