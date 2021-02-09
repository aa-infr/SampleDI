using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;

namespace Infrabel.ICT.Framework.Service
{
    public interface IUserIdentity
    {
        CultureInfo Culture { get; }
        string LanguageIsoCode { get; }
        string Name { get; }

        IEnumerable<Claim> GetClaims(Func<Claim, bool> predicate);

        bool HasClaim(Func<Claim, bool> predicate);

        bool HasClaim(string type, string value = "");

        IEnumerable<string> GetClaimValues(string type);

        IEnumerable<string> GetRoles();

        Maybe<string> GetClaimStringValue(string type);

        Maybe<bool> GetClaimBooleanValue(string type);

        Maybe<long> GetClaimNumberValue(string type);

        bool IsInRolePolicy(string rolePolicy);
    }
}