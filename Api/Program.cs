using System;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Infrabel.ICT.Framework.Extended.AspNetCore.Extension;
using Infrabel.ICT.Framework.Extended.AspNetCore.Resolver;
using Infrabel.ICT.Framework.Extended.DryIoc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Reflection;

namespace ICT.Template.Api
{
    public class Program
    {
        public static int Main(string[] args)
        {

            try
            {
                Log.Information("Starting web host");
                var host = CreateHostBuilder(args)
                    .Build();


                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var container = new Container();

            return Host.CreateDefaultBuilder(args)
                       .UseServiceProviderFactory(context => new DryIocServiceProviderFactory(container))
                       .ConfigureWebHostDefaults(webBuilder =>
                       {
                           webBuilder.ConfigureServices((context, services) =>
                               {
                                   var optionsResolver = new OptionsResolver(context.Configuration);
                                   var connectionStringResolver = new ConnectionStringResolver(context.Configuration);
                                   DryIocBootstrapper.GetInstance()
                                       .Initialize(container, optionsResolver, connectionStringResolver)
                                       .LoadModules<Infrabel.ICT.Framework.RegistrationModule>()
                                       .LoadModules<Infrabel.ICT.Framework.Extended.AspNetCore.RegistrationModule>()
                                       .LoadModules<Infrabel.ICT.Framework.Extended.EntityFramework.RegistrationModule>()
                                       .LoadModules<ICT.Template.Api.RegistrationModule>()
                                       .BuildContainer();
                               })
                               .ConfigureKestrelListening()
                               .UseSerilog((builderContext, loggerConfig) =>
                               {
                                   loggerConfig.ReadFrom.Configuration(builderContext.Configuration)
                                       .Enrich.FromLogContext();
                               })
                               .UseStartup<Startup>()
                               .UseSetting(WebHostDefaults.ApplicationKey, Assembly.GetExecutingAssembly().FullName);
                       });
        }
    }
}