using Infrabel.ICT.Framework.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Context
{
    public class UserContext : IUserContext
    {
        private static readonly CultureInfo FallbackCulture = new CultureInfo("nl");
        private static readonly ClaimsPrincipal FallbackUser = BuildFallbackPrincipal();
        private readonly Func<HttpContext> _contextFactory;
        private readonly IRolePoliciesService _rolePoliciesService;

        public UserContext(Func<HttpContext> contextFactory, IRolePoliciesService rolePoliciesService, IDateService dateService)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _rolePoliciesService = rolePoliciesService ?? throw new ArgumentNullException(nameof(rolePoliciesService));
        }

        private static ClaimsPrincipal GetClaimsPrincipal(HttpContext context)
        {
            if (context?.User?.Identity?.IsAuthenticated != true)
                return FallbackUser;

            return context.User;
        }

        private static ClaimsPrincipal BuildFallbackPrincipal()
        {
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.Name, WindowsIdentity.GetCurrent().Name));
            return new ClaimsPrincipal(identity);
        }

        private static CultureInfo GetCulture(HttpContext context)
        {
            if (context == null)
                return FallbackCulture;

            var cultureFeature = context.Features.Get<IRequestCultureFeature>();

            return cultureFeature?.RequestCulture?.Culture ?? FallbackCulture;
        }

        public void ClearUserIdentity()
        {
            var context = _contextFactory();
        }

    public IUserIdentity Identity
    {
      get
      {
        var context = _contextFactory();


          var principal = GetClaimsPrincipal(context);
          var culture = GetCulture(context);

          return UserIdentity.Create(principal, culture,
              (rolePolicy, roles) => _rolePoliciesService.IsInRolePolicy(rolePolicy, roles));
      }
    }
  }
}