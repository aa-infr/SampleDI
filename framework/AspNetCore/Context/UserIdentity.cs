using Infrabel.ICT.Framework.Extension;
using Infrabel.ICT.Framework.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Context
{
    public class UserIdentity : IUserIdentity
    {
        public CultureInfo Culture { get; }
        public string LanguageIsoCode { get; }
        public string Name { get; }
        private readonly IDictionary<string, IEnumerable<Claim>> _claims;
        private readonly ISet<string> _roles;
        private readonly Func<string, IEnumerable<string>, bool> _rolePolicyPredicate;

        private UserIdentity(string name, CultureInfo culture, ISet<string> roles, IDictionary<string, IEnumerable<Claim>> claims, Func<string, IEnumerable<string>, bool> rolePolicyPredicate)
        {
            Name = name ?? string.Empty;
            Culture = culture ?? throw new ArgumentNullException(nameof(culture));
            _claims = claims ?? throw new ArgumentNullException(nameof(claims));
            _roles = roles ?? throw new ArgumentNullException(nameof(roles));
            _rolePolicyPredicate = rolePolicyPredicate ?? throw new ArgumentNullException(nameof(rolePolicyPredicate));
            LanguageIsoCode = Culture.TwoLetterISOLanguageName;
        }

        public IEnumerable<Claim> GetClaims(Func<Claim, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return _claims.SelectMany(c => c.Value)
                         .Where(predicate);
        }

        public bool HasClaim(Func<Claim, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            return GetClaims(predicate).Any();
        }

        public bool HasClaim(string type, string value = "")
        {
            if (string.IsNullOrWhiteSpace(type) || !_claims.ContainsKey(type))
                return false;

            return true;
        }

        public IEnumerable<string> GetClaimValues(string type)
        {
            if (string.IsNullOrWhiteSpace(type) || !_claims.ContainsKey(type))
                return new string[] { };

            return _claims[type].Select(v => v.Value);
        }

        public IEnumerable<string> GetRoles()
        {
            return _roles.AsEnumerable();
        }

        public Maybe<string> GetClaimStringValue(string type)
        {
            var result = GetClaimValues(type).SingleOrDefault();

            return result == null ? Maybe<string>.WithNoValue() : Maybe<string>.WithValue(result);
        }

        public Maybe<bool> GetClaimBooleanValue(string type)
        {
            var result = GetClaimValues(type)

                                             .Select(v => bool.TryParse(v, out var convertedValue) ? new { converted = true, convertedValue } : new { converted = true, convertedValue = false })
                                             .SingleOrDefault(c => c.converted);

            return result == null ? Maybe<bool>.WithNoValue() : Maybe<bool>.WithValue(result.convertedValue);
        }

        public Maybe<long> GetClaimNumberValue(string type)
        {
            var result = GetClaimValues(type)
                .Select(v => long.TryParse(v, out var convertedValue) ? new { converted = true, convertedValue } : new { converted = true, convertedValue = 0L })
                .FirstOrDefault(c => c.converted);

            return result == null ? Maybe<long>.WithNoValue() : Maybe<long>.WithValue(result.convertedValue);
        }

        public bool IsInRolePolicy(string rolePolicy)
        {
            return _rolePolicyPredicate(rolePolicy, _roles);
        }

        private static IDictionary<string, IEnumerable<Claim>> GetAuthenticatedClaimsMap(ClaimsPrincipal principal)
        {
            var authenticatedClaims = principal.Identities?.Where(i => i?.IsAuthenticated ?? false)
                .SelectMany(i => i.Claims)
                .Where(c => !string.IsNullOrEmpty(c?.Value)) ?? new Claim[] { };

            return authenticatedClaims.ToLookup(c => c.Type, c => c, StringComparer.OrdinalIgnoreCase)
                                      .ToDictionary(c => c.Key, c => c.AsEnumerable(), StringComparer.OrdinalIgnoreCase);
        }

        private static ISet<string> GetRoles(ClaimsPrincipal principal)
        {
            var result = principal.Identities?.Where(i => i?.IsAuthenticated ?? false)
                .SelectMany(i => i.Claims, (i, c) => new { i.RoleClaimType, Claim = c })
                .Where(c => c.Claim != null)
                .Where(c => !string.IsNullOrWhiteSpace(c.RoleClaimType) &&
                            string.Equals(c.RoleClaimType, c.Claim.Type) ||
                            string.Equals(c.Claim.Type, ClaimTypes.Role))
                .Select(c => c.Claim.Value) ?? new string[] { };

            return result.ToHashSet();
        }

        private static string GetName(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null)
                return string.Empty;

            return (claimsPrincipal.Claims?.Where(c => c != null)
                .OrderByDescending(c => c.Subject?.IsAuthenticated ?? false)
                .Where(c => string.Equals(c.Type, c.Subject?.NameClaimType ?? ClaimTypes.Name))
                .Select(c => c.Value)
                .FirstOrDefault() ?? string.Empty).ExtractUserName();
        }

        public static UserIdentity Create(ClaimsPrincipal principal, CultureInfo culture, Func<string, IEnumerable<string>, bool> rolePolicyPredicate)
        {
            var name = GetName(principal);
            var roles = GetRoles(principal);
            var claims = GetAuthenticatedClaimsMap(principal);
            return new UserIdentity(name, culture, roles, claims, rolePolicyPredicate);
        }
    }
}