using Infrabel.ICT.Framework.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Resolver
{
    public class ConnectionStringResolver : ConnectionStringResolverBase
    {
        private readonly IConfiguration _configuration;

        public ConnectionStringResolver(IConfiguration configuration)
        {
            ChangeToken.OnChange(configuration.GetReloadToken, InvalidateCache, this);
            _configuration = configuration;
        }

        protected override string BuildConnectionString(string name)
        {
            var result = _configuration.GetConnectionString(name);
            if (string.IsNullOrWhiteSpace(result))
                throw new InvalidOperationException($"The connection string for {name} is undefined");

            return result;
        }
    }
}