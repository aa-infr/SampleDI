using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Context
{
    public class IdentityComparer : IEqualityComparer<IIdentity>
    {
        public bool Equals(IIdentity x, IIdentity y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            return string.Equals(x.AuthenticationType, y.AuthenticationType, StringComparison.OrdinalIgnoreCase) &&
                                 x.IsAuthenticated == y.IsAuthenticated &&
                                 string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(IIdentity obj)
        {
            return HashCode.Combine(obj.AuthenticationType, obj.IsAuthenticated, obj.Name);
        }
    }
}