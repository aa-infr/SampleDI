using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Option
{
    public class FederationAuthConfig : AuthConfigBase
    {
        public string MetadataAddress { get; set; }
        public string Authority { get; set; }
        public string Issuer { get; set; }
        public List<string> Audiences { get; set; } = new List<string>();
        public override string AuthenticationScheme => JwtBearerDefaults.AuthenticationScheme;
    }
}