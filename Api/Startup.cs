using DryIoc;
using ICT.Template.Api.Services;
using Infrabel.ICT.Framework.Extended.AspNetCore.Authorization;
using Infrabel.ICT.Framework.Extended.AspNetCore.Extension;
using Infrabel.ICT.Framework.Extended.AspNetCore.Middleware;
using Infrabel.ICT.Framework.Extended.AspNetCore.Option;
using Infrabel.ICT.Framework.Extended.AspNetCore.Resolver;
using Infrabel.ICT.Framework.Extended.DryIoc;
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
            DryIocBootstrapper.GetInstance()
                .Initialize(container, _optionsResolver, _connectionStringResolver)
                .LoadModules<Infrabel.ICT.Framework.RegistrationModule>()
                .LoadModules<Infrabel.ICT.Framework.Extended.AspNetCore.RegistrationModule>()
                .LoadModules<Infrabel.ICT.Framework.Extended.EntityFramework.RegistrationModule>()
                .LoadModules<ICT.Template.Api.RegistrationModule>()
                .BuildContainer();


          // Replace the built-in IControllerActivator with a custom one that forwards the call to DryIoc.
          services.AddSingleton<IControllerActivator>(new DryIocControllerActivator(container));


          services.AddControllers(_customMediaTypeService, _optionsResolver.Resolve<JsonOptions>);
          services.AddTransient<IClaimsTransformation, ClaimsRefinementTransformation>();

          services.AddMemoryCache()
              .AddCors()
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

            app
               .UseForwardersConfiguration(_optionsResolver.Resolve<KestrelOptions>)
               .UseCorsConfiguration(_optionsResolver.Resolve<CorsOptions>)
               .UseHttpsRedirection()
               .UseRouting()
               .UseRequestLocalization(_optionsResolver.Resolve<LocalizationOptions>)
               .UseResponseCompression(_optionsResolver.Resolve<CompressionOptions>)
               .UseEndpoints(endpoints =>
                 {
                     endpoints.MapControllers();
                 });
        }
    }
}