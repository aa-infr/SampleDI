using Infrabel.ICT.Framework.Extended.AspNetCore.Option;
using Infrabel.ICT.Framework.Extension;
using Infrabel.ICT.Framework.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonOptions = Infrabel.ICT.Framework.Extended.AspNetCore.Option.JsonOptions;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, Func<AuthenticationAndAuthorizationOptions> authOptionsFunc)
        {
            var authOptions = authOptionsFunc();

            if (authOptions.IsWindowsAuthenticationEnabled)
            {
                var windowsAuthConfig = authOptions.WindowsAuthentication;
                services.AddWindowsAuthentication(windowsAuthConfig);
            }
            if (authOptions.IsAdfsAuthenticationEnabled)
            {
                var adfsAuthConfig = authOptions.AdfsAuthentication;
                services.AddAdfs(adfsAuthConfig);
            }

            var schemes = authOptions.Schemes.ToArray();
            services.AddDefaultPolicy(schemes);
            services.AddRolePolicies(authOptions.RolePolicies, schemes);

            return services;
        }

        private static void AddAdfs(this IServiceCollection services, FederationAuthConfig adfsAuthConfig)
        {
            if (adfsAuthConfig == null)
                throw new ArgumentNullException(nameof(adfsAuthConfig));
            services.AddAuthentication(adfsAuthConfig.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  options.Authority = adfsAuthConfig.Authority;
                  options.RequireHttpsMetadata = false;

                  options.TokenValidationParameters = new TokenValidationParameters()
                  {
                      ValidateIssuer = true,
                      ValidIssuer = adfsAuthConfig.Issuer,
                  };

                  var audiencesCount = adfsAuthConfig.Audiences.Count;
                  if (audiencesCount > 1)
                  {
                      options.TokenValidationParameters.ValidAudiences = adfsAuthConfig.Audiences.ToList();
                      options.TokenValidationParameters.ValidateAudience = true;
                  }
                  else if (audiencesCount == 1)
                  {
                      options.TokenValidationParameters.ValidAudience = adfsAuthConfig.Audiences.Single();
                      options.TokenValidationParameters.ValidateAudience = true;
                  }
                  else
                  {
                      options.TokenValidationParameters.ValidateAudience = false;
                  }
              });
        }

        private static void AddWindowsAuthentication(this IServiceCollection services, WindowsAuthConfig options)
        {
            var authenticationBuilder = services.AddAuthentication(options.AuthenticationScheme);

            if (options.IsSelfHosted)
                authenticationBuilder.AddNegotiate();
        }

        private static void AddDefaultPolicy(this IServiceCollection services, string[] schemes)
        {
            var authenticatedPolicyBuilder = new AuthorizationPolicyBuilder();

            if (schemes.Any())
            {
                authenticatedPolicyBuilder = authenticatedPolicyBuilder.AddAuthenticationSchemes(schemes);
                authenticatedPolicyBuilder = authenticatedPolicyBuilder.RequireAuthenticatedUser();
            }
            else
            {
                authenticatedPolicyBuilder = authenticatedPolicyBuilder.RequireAssertion(_ => true);
            }

            services.AddAuthorization(o => o.DefaultPolicy = authenticatedPolicyBuilder.Build());
        }

        private static void AddRolePolicies(this IServiceCollection services, IList<RolePolicy> rolePolicies, string[] schemes)
        {
            if (!(rolePolicies?.Any() ?? false))
                return;

            rolePolicies.Execute(r =>
            {
                if (r.Groups.Any())
                {
                    services.AddAlternativeRolesPolicy(r.Name, r.Groups.ToArray(), schemes);
                }
            });
        }

        private static void AddAlternativeRolesPolicy(this IServiceCollection services, string policyName, string[] roles, string[] schemes)
        {
            var rolePolicyBuilder = new AuthorizationPolicyBuilder();

            if (schemes.Any())
            {
                rolePolicyBuilder = rolePolicyBuilder.AddAuthenticationSchemes(schemes);
                rolePolicyBuilder = rolePolicyBuilder.RequireAuthenticatedUser();
                rolePolicyBuilder = rolePolicyBuilder.RequireAssertion(ctx =>
                    ctx?.User?.HasClaim(c =>
                        string.Equals(c.Type, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase) &&
                        roles.Any(r => string.Equals(r, c.Value, StringComparison.OrdinalIgnoreCase))) ??
                    false);
            }
            else
            {
                rolePolicyBuilder = rolePolicyBuilder.RequireAssertion(_ => true);
            }

            services.AddAuthorization(o =>
            {
                o.AddPolicy(policyName, rolePolicyBuilder.Build());
            });
        }

        public static IServiceCollection AddCompression(this IServiceCollection services, Func<CompressionOptions> optionsFunc, ICustomMediaTypeService customMediaTypeService = null)
        {
            var options = optionsFunc();

            if (!options.EnableCompression)
                return services;

            services.AddResponseCompression(o =>
            {
                o.EnableForHttps = options.EnableForHttps;

                if (customMediaTypeService != null)
                    o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(customMediaTypeService.GetAll());
                else
                    o.MimeTypes = ResponseCompressionDefaults.MimeTypes;
            });

            return services;
        }

        public static IMvcBuilder AddControllers(this IServiceCollection services, ICustomMediaTypeService customMediaTypeService = null, Func<JsonOptions> optionsFunc = null)
        {
            var options = optionsFunc == null ? new JsonOptions() : optionsFunc();

            var builder = services.AddControllers(o =>
            {
                o.ConfigureNotAcceptableMediaTypes();
                o.AddGlobalActionFilters();
            });

            if (options.UseNewtonSoftJson)
                return builder.ConfigureNewtonSoftJson(options, customMediaTypeService);
            return builder.ConfigureTextJson(options, customMediaTypeService);
        }

        public static IMvcBuilder ConfigureNewtonSoftJson(this IMvcBuilder builder, JsonOptions options, ICustomMediaTypeService customMediaTypeService)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return builder.AddNewtonsoftJson(o =>
            {
                if (options.UseCamelCase)
                    o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                if (options.IgnoreNullValue)
                    o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                if (options.EnumAsString)
                    o.SerializerSettings.Converters.Add(new StringEnumConverter());
                if (options.SerializeReferenceLoop)
                    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            }).AddMvcOptions(o =>
            {
                o.AddCustomInputFormatters();
                o.AddCustomOutputFormatters<NewtonsoftJsonOutputFormatter>(customMediaTypeService);
            });
        }

        public static IMvcBuilder ConfigureTextJson(this IMvcBuilder builder, JsonOptions options, ICustomMediaTypeService customMediaTypeService)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return builder.AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.AllowTrailingCommas = options.BeTolerant;
                o.JsonSerializerOptions.PropertyNameCaseInsensitive = options.BeTolerant;
                if (options.BeTolerant)
                    o.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                o.JsonSerializerOptions.IgnoreNullValues = options.IgnoreNullValue;
                o.JsonSerializerOptions.PropertyNamingPolicy = options.UseCamelCase ? JsonNamingPolicy.CamelCase : null;
                if (options.EnumAsString)
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }).AddMvcOptions(o =>
            {
                o.AddCustomInputFormatters();
                o.AddCustomOutputFormatters<SystemTextJsonOutputFormatter>(customMediaTypeService);
            });
        }

        public static MvcOptions ConfigureNotAcceptableMediaTypes(this MvcOptions options)
        {
            options.ReturnHttpNotAcceptable = true;
            return options;
        }

        public static MvcOptions AddGlobalActionFilters(this MvcOptions options)
        {
            return options;
        }

        public static MvcOptions AddCustomInputFormatters(this MvcOptions options)
        {
            var xmlInputFormatter = new XmlDataContractSerializerInputFormatter(options);
            options.InputFormatters.Add(xmlInputFormatter);

            return options;
        }

        public static MvcOptions AddCustomOutputFormatters<TJsonOutputFormatter>(this MvcOptions options, ICustomMediaTypeService customMediaTypeService) where TJsonOutputFormatter : OutputFormatter
        {
            var xmlOutputFormatter = new XmlDataContractSerializerOutputFormatter();

            options.OutputFormatters.Add(xmlOutputFormatter);

            if (customMediaTypeService == null)
                return options;

            customMediaTypeService.GetXmlTypes().Execute(e => xmlOutputFormatter.SupportedMediaTypes.Add(e));

            var jsonOutputFormatter = options.OutputFormatters
                                             .OfType<TJsonOutputFormatter>()
                                             .First();

            customMediaTypeService.GetJsonTypes().Execute(e => jsonOutputFormatter.SupportedMediaTypes.Add(e));
            return options;
        }
    }
}