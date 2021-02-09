using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Option
{
    public class AuthenticationAndAuthorizationOptions
    {
        public FederationAuthConfig AdfsAuthentication { get; set; } = new FederationAuthConfig();
        public WindowsAuthConfig WindowsAuthentication { get; set; } = new WindowsAuthConfig();
        public bool IsWindowsAuthenticationEnabled => WindowsAuthentication.IsEnabled;
        public bool IsAdfsAuthenticationEnabled => AdfsAuthentication.IsEnabled;

        public List<RolePolicy> RolePolicies { get; set; } = new List<RolePolicy>();

        public IEnumerable<string> Schemes
        {
            get
            {
                if (IsAdfsAuthenticationEnabled)
                    yield return AdfsAuthentication.AuthenticationScheme;
                if (IsWindowsAuthenticationEnabled)
                    yield return WindowsAuthentication.AuthenticationScheme;
            }
        }
    }
}