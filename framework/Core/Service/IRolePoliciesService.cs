using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Service
{
    public interface IRolePoliciesService
    {
        bool IsInRolePolicy(string rolePolicy, IEnumerable<string> roles);
    }
}