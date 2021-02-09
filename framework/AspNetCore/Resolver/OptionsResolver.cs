using Infrabel.ICT.Framework.Extended.AspNetCore.Extension;
using Infrabel.ICT.Framework.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Resolver
{
    public class OptionsResolver : OptionsResolverBase
    {
        private readonly IConfiguration _configuration;

        public OptionsResolver(IConfiguration configuration)
        {
            ChangeToken.OnChange(configuration.GetReloadToken, InvalidateCache, this);
            _configuration = configuration;
        }

        protected override TOptions BuildOptions<TOptions>()
        {
            return _configuration.GetOptions<TOptions>();
        }
    }
}