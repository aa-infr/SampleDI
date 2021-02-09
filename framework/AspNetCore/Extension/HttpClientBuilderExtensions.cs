using Infrabel.ICT.Framework.Extended.AspNetCore.Option;
using Infrabel.ICT.Framework.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Extension
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddHttpClientWithProxy(this IServiceCollection services, string name)
        {
            return services.AddHttpClient(name)
                           .ConfigurePrimaryHttpMessageHandler(BuildProxyConfigurationHttpHandler);
        }

        private static HttpClientHandler BuildProxyConfigurationHttpHandler(IServiceProvider provider)
        {
            var options = provider.GetService<IOptionsResolver>().Resolve<HttpClientProxyOptions>();
            var handler = new HttpClientHandler();

            if (options.EnableFollowRedirect)
                handler.AllowAutoRedirect = true;
            if (options.EnableCompression)
                handler.AutomaticDecompression = DecompressionMethods.All;

            if (!options.EnableProxy)
                return handler;

            handler.UseProxy = true;

            if (!string.IsNullOrWhiteSpace(options.ProxyUrl))
                handler.Proxy = new WebProxy(new Uri(options.ProxyUrl), true);

            if (string.IsNullOrWhiteSpace(options.UserName))
                return handler;

            ICredentials credentials = !string.IsNullOrWhiteSpace(options.Domain) ? new NetworkCredential(options.UserName, options.Password, options.Domain)
                : new NetworkCredential(options.UserName, options.Password);

            handler.DefaultProxyCredentials = credentials;
            handler.Credentials = credentials;
            return handler;
        }
    }
}