using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MassTransit;
using MassTransit.Transports.InMemory;
using WebApplication.Messages;

namespace WebApplication
{
    public class TraceMessageObserver : IObserver<ConsumeContext<TraceMessage>>
    {
        public void OnNext(ConsumeContext<TraceMessage> context)
        {


            Console.WriteLine("Trace: {0}", context.Message.Message);
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }
    }

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            
            var bus = Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.UseJsonSerializer();
                cfg.SetHost(Program.Host);

                cfg.ReceiveEndpoint("in_process_queue", ep =>
                {
                    ep.Observer<TraceMessage>(new TraceMessageObserver());

                });
            });

            bus.Start();

            services.AddSingleton<IBus>(bus);

            services.Configure<IISOptions>(options => {
     
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
