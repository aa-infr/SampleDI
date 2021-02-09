using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Extension
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddLocalization<TResource>(this IMvcBuilder builder, string resourcePath) where TResource : class, new()
        {
            return builder.AddMvcLocalization(o =>
                {
                    if (!string.IsNullOrWhiteSpace(resourcePath))
                        o.ResourcesPath = resourcePath;
                })
                .AddDataAnnotationsLocalization(o => o.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(TResource)));
        }

    }
}