using System;
using System.Collections.Generic;
using System.Linq;
using Infrabel.ICT.Framework.Extended.AspNetCore.Option;
using Infrabel.ICT.Framework.Extension;
using Infrabel.ICT.Framework.Ioc;
using Infrabel.ICT.Framework.Service;

namespace Infrabel.ICT.Framework.Extended.AspNetCore.Service
{
    [IoCRegistration(RegistrationLifeTime.Singleton)]
    public class RolePoliciesService : IRolePoliciesService
    {
        private readonly Lazy<IDictionary<string, ISet<string>>> _lazyDictionary;

        public RolePoliciesService(IOptionsResolver<AuthenticationAndAuthorizationOptions> authorizationResolver)
        {
            if (authorizationResolver == null)
                throw new ArgumentNullException(nameof(authorizationResolver));

            var options = authorizationResolver.Resolve();

            _lazyDictionary = new Lazy<IDictionary<string, ISet<string>>>(BuildDictionary(options?.RolePolicies));
        }

        private IDictionary<string, ISet<string>> Dictionary => _lazyDictionary.Value;

        public bool IsInRolePolicy(string rolePolicy, IEnumerable<string> roles)
        {
            if (!Dictionary.ContainsKey(rolePolicy))
                throw new ArgumentOutOfRangeException(nameof(rolePolicy));

            return roles?.Any(r => Dictionary[rolePolicy].Contains(r)) ?? false;
        }

        private IDictionary<string, ISet<string>> BuildDictionary(IEnumerable<RolePolicy> policies)
        {
            var result = new Dictionary<string, ISet<string>>();
            policies.Execute(p =>
            {
                if (!result.ContainsKey(p.Name))
                    result.Add(p.Name, new HashSet<string>(StringComparer.OrdinalIgnoreCase));

                p.Groups.Execute(g =>
                {
                    if (!result[p.Name].Contains(g))
                        result[p.Name].Add(g);
                });
            });

            return result;
        }
    }
}