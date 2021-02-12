using SimpleInjector;
using ICT.Template.Api.Services;
using Infrabel.ICT.Framework.Extended.AspNetCore.Authorization;
using Infrabel.ICT.Framework.Extended.AspNetCore.Extension;
using Infrabel.ICT.Framework.Extended.AspNetCore.Middleware;
using Infrabel.ICT.Framework.Extended.AspNetCore.Option;
using Infrabel.ICT.Framework.Extended.AspNetCore.Resolver;
using Infrabel.ICT.Framework.Extended.SimpleInjectorIoc;
using Infrabel.ICT.Framework.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using JsonOptions = Infrabel.ICT.Framework.Extended.AspNetCore.Option.JsonOptions;
using SimpleInjector.Lifestyles;
using Infrabel.ICT.Framework.Extended.EntityFramework.Configuration;
using System.Collections.Generic;
using System;
using Infrabel.ICT.Framework.Extended.EntityFramework;
using ICT.Template.Infrastructure.Data;
using Infrabel.ICT.Framework.Entity;
using ICT.Template.Api.Controllers;
using ICT.Template.Core.Services;
using ICT.Template.Infrastructure.Services;
using System.Reflection;

namespace ICT.Template.Api
{
    public class Startup
    {
        private readonly OptionsResolver _optionsResolver;
        private readonly ICustomMediaTypeService _customMediaTypeService;
        private readonly ConnectionStringResolver _connectionStringResolver;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _optionsResolver = new OptionsResolver(configuration);
            _connectionStringResolver = new ConnectionStringResolver(configuration);
            _customMediaTypeService = new CustomMediaTypeService();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var container = new Container();
            // Default lifestyle scoped + async
            // The recommendation is to use AsyncScopedLifestyle in for applications that solely consist of a Web API(or other asynchronous technologies such as ASP.NET Core)
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            //container.Options.AllowOverridingRegistrations = true;
            //container.Options.EnableAutoVerification = false;

            container.Register<SamplesController>();
            container.RegisterInstance<Func<IEnumerable<EntityBaseConfiguration>>>(() => container.GetAllInstances<EntityBaseConfiguration>());
            container.Collection.Register<EntityBaseConfiguration>(Assembly.GetExecutingAssembly());

            SimpleInjectorIocBootstrapper.GetInstance()
                    .Initialize(container, _optionsResolver, _connectionStringResolver)
                    .LoadModules<Infrabel.ICT.Framework.RegistrationModule>()
                    .LoadModules<Infrabel.ICT.Framework.Extended.AspNetCore.RegistrationModule>()
                    .LoadModules<Infrabel.ICT.Framework.Extended.EntityFramework.RegistrationModule>()
                    .LoadModules<ICT.Template.Infrastructure.RegistrationModule>()
                    .LoadModules<ICT.Template.Api.RegistrationModule>()
                    .BuildContainer();


            // Replace the built-in IControllerActivator with a custom one that forwards the call to SimpleInjectorIoc.
            services.AddSingleton<IControllerActivator>(new SimpleInjectorIocControllerActivator(container));

            // Wrap AspNet requests into Simpleinjector's scoped lifestyle
            services.UseSimpleInjectorAspNetRequestScoping(container);

            services.AddControllers(_customMediaTypeService, _optionsResolver.Resolve<JsonOptions>);


            services.AddSingleton<IEnvironmentInfoService>(new EnvironmentInfoService());
            //services.AddTransient<IClaimsTransformation, ClaimsRefinementTransformation>();

            services.AddMemoryCache()
                .AddCors()
                .AddAuthenticationAndAuthorization(_optionsResolver.Resolve<AuthenticationAndAuthorizationOptions>)
                .Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; })
                .AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "ICT-Template API", Version = "v1" }))
                .AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
          if (env.IsDevelopment())
          {
            app.UseDeveloperExceptionPage()
               .UseSwagger()
               .UseSwaggerUI(c =>
               {
                 c.SwaggerEndpoint("/swagger/v1/swagger.json", "ICT-Template API");
                 c.RoutePrefix = string.Empty;
               });
          }

          app.UseAuthentication()
             .UseMiddleware<LoggingMiddleware>()
             .UseForwardersConfiguration(_optionsResolver.Resolve<KestrelOptions>)
             .UseCorsConfiguration(_optionsResolver.Resolve<CorsOptions>)
             .UseHttpsRedirection()
             .UseRouting()
             .UseAuthorization()
             .UseRequestLocalization(_optionsResolver.Resolve<LocalizationOptions>)
             .UseResponseCompression(_optionsResolver.Resolve<CompressionOptions>)
             .UseEndpoints(endpoints =>
             {
               endpoints.MapControllers();
             });
        }
  }
}