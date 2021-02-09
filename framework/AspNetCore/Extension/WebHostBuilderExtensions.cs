using Infrabel.ICT.Framework.Extended.AspNetCore.Option;
using Infrabel.ICT.Framework.Extension;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Linq;
using System.Net;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Extension
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder ConfigureKestrelListening(this IWebHostBuilder builder)
        {
            builder.ConfigureKestrel((context, options) =>
            {
                var kestrelOptions = context.Configuration.GetOptions<KestrelOptions>();

                if (!kestrelOptions.EnableKestrel || !kestrelOptions.IsPortValid)
                    return;

                if (kestrelOptions.EnableAnyIpListening)
                {
                    options.ListenAnyIP(kestrelOptions.ListeningPort, EnableHttps);
                    return;
                }

                if (kestrelOptions.ListeningIps?.Any() ?? false)
                    kestrelOptions.ListeningIps.Execute(i =>
                        options.Listen(IPAddress.Parse(i), kestrelOptions.ListeningPort, EnableHttps));

                if (kestrelOptions.EnableLocalHostListening)
                    options.ListenLocalhost(kestrelOptions.ListeningPort, EnableHttps);

                void EnableHttps(ListenOptions o)
                {
                    if (kestrelOptions.EnableHttps) o.UseHttps();
                }
            });

            return builder;
        }
    }
}