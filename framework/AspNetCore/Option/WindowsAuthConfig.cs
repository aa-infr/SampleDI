using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Server.IIS;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Option
{
    public class WindowsAuthConfig : AuthConfigBase
    {
        public bool IsSelfHosted { get; set; }
        public override string AuthenticationScheme => IsSelfHosted ? NegotiateDefaults.AuthenticationScheme : IISServerDefaults.AuthenticationScheme;
    }
}