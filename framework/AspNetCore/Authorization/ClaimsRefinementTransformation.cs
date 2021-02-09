using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrabel.ICT.Framework.Extension;
using Infrabel.ICT.Framework.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Authorization
{
    public class ClaimsRefinementTransformation : IClaimsTransformation
    {
        private readonly Func<IIdentityClaimRefiner[]> _refiners;
        private readonly IDateService _dateService;
        private static readonly string RefinedClaimType = "ClaimsRefined";
        private readonly ILogger<ClaimsRefinementTransformation> _logger;

        public ClaimsRefinementTransformation(Func<IIdentityClaimRefiner[]> refiners, IDateService dateService, ILogger<ClaimsRefinementTransformation> logger)
        {
            _refiners = refiners ?? throw new ArgumentNullException(nameof(refiners));
            _dateService = dateService ?? throw new ArgumentNullException(nameof(dateService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            foreach (ClaimsIdentity claimIdentity in principal?.Identities)
            {
                if (string.IsNullOrWhiteSpace(claimIdentity.Name))
                {
                    string fullName = claimIdentity?.Claims?.FirstOrDefault(c => c.Type == "fullname")?.Value;
                    _logger.LogError($"The claimIdentity.Name propery is empty for the person {fullName}. Probably (s)he doen't have the role OREE Infrabel." +
                        $" As a consequence classes implementing IIdentityClaimRefiner will not be called.");
                }
            }

            var identities = principal?.Identities?.Where(i => (i?.IsAuthenticated ?? false) && !string.IsNullOrWhiteSpace(i.Name));

            if (identities == null)
                return principal;

            var refiners = _refiners();
            if (!refiners.Any())
                return principal;
            await identities.ExecuteAsync(async i => await RefineIdentityAsync(i, refiners));

            return principal;
        }

        private async Task RefineIdentityAsync(ClaimsIdentity identity, IIdentityClaimRefiner[] refiners)
        {
            if (identity?.HasClaim(c => string.Equals(c?.Type, RefinedClaimType, StringComparison.OrdinalIgnoreCase)) ?? true)
                return;

            await refiners.ExecuteAsync(async c => {
                try
                {
                    await c.RefineAsync(identity);
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
          
            });

            identity.AddClaim(new Claim(RefinedClaimType, _dateService.UtcNow
                                                                      .ToIso8601()));
        }
    }
}