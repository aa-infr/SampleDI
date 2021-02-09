using Infrabel.ICT.Framework.Extended.AspNetCore.Option;
using Infrabel.ICT.Framework.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Extension
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseResponseCompression(this IApplicationBuilder appBuilder, Func<CompressionOptions> optionsFunc)
        {
            var options = optionsFunc();

            if (!options.EnableCompression)
                return appBuilder;

            appBuilder.UseResponseCompression();

            return appBuilder;
        }

        public static IApplicationBuilder UseCorsConfiguration(this IApplicationBuilder appBuilder, Func<CorsOptions> optionsFunc)
        {
            var options = optionsFunc();

            if (!options.EnableCors)
                return appBuilder;

            if (options.AllowedOrigins.Any())
                appBuilder.UseCors(o => o.WithOrigins(options.AllowedOrigins.ToArray())
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
            else
                appBuilder.UseCors(o => o.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            return appBuilder;
        }

        public static IApplicationBuilder UseForwardersConfiguration(this IApplicationBuilder appBuilder, Func<KestrelOptions> optionsFunc)
        {
            var options = optionsFunc();

            if (!options.UseForwarders)
                return appBuilder;

            var forwardedHeadersOptions = new ForwardedHeadersOptions()
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            };

            if (options.ValidForwarders?.Any() ?? false)
                options.ValidForwarders.Execute(i => forwardedHeadersOptions.KnownProxies.Add(IPAddress.Parse(i)));

            appBuilder.UseForwardedHeaders(forwardedHeadersOptions);

            return appBuilder;
        }

        private static readonly Lazy<List<CultureInfo>> FallbackCultures = new Lazy<List<CultureInfo>>(() => new List<CultureInfo> { new CultureInfo("fr"), new CultureInfo("en"), new CultureInfo("nl") });

        public static IApplicationBuilder UseRequestLocalization(this IApplicationBuilder appBuilder, Func<LocalizationOptions> optionsFunc)
        {
            var options = optionsFunc();

            var supportedCultures = options.SupportedCultures.Select(c => new CultureInfo(c)).ToList();

            if (!(supportedCultures?.Any() ?? false))
                supportedCultures = FallbackCultures.Value;

            appBuilder.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(supportedCultures.First()),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });

            return appBuilder;
        }
    }
}